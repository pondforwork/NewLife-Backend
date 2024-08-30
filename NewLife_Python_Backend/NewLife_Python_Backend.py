from dotenv import load_dotenv
from flask import Flask, request, jsonify
import cv2
import numpy as np
import json
import os
import tempfile
import mysql.connector

app = Flask(__name__)

load_dotenv()

# Retrieve environment variables
DB_HOST = os.getenv('DB_HOST')
DB_PORT = os.getenv('DB_PORT')
DB_USER = os.getenv('DB_USER')
DB_PASSWORD = os.getenv('DB_PASSWORD')
DB_NAME = os.getenv('DB_NAME')


def get_db_connection():
    return mysql.connector.connect(
        host=DB_HOST,
        port=DB_PORT, 
        user=DB_USER,
        password=DB_PASSWORD,
        database=DB_NAME
    )

@app.route('/process_image', methods=['POST'])
def process_image():
    # Check if the request contains an image file
    if 'image' not in request.files:
        return jsonify({"error": "No image uploaded"}), 400
    if 'imageId' not in request.form:
        return jsonify({"error": "No text content provided"}), 400

    # Get the image file from the request
    file = request.files['image']
    
    imageId = request.form['imageId']


    # Create a temporary file to store the uploaded image
    with tempfile.NamedTemporaryFile(delete=False) as temp_file:
        file_path = temp_file.name
        file.save(file_path)

    try:
        # Load the image using OpenCV
        image = cv2.imread(file_path)

        # Convert the image to grayscale
        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

        # Apply Canny edge detection
        edges = cv2.Canny(gray, 50, 150)

        # Find contours (which are the vectorized data)
        contours, _ = cv2.findContours(edges, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        # Convert contours to a serializable format (list of points)
        contours_list = [contour.tolist() for contour in contours]

        # Serialize the contours as a JSON string
        contours_json = json.dumps(contours_list)

        connection = get_db_connection()
        update_image_status(imageId)
        insertImageData(imageId,contours_json)
        return jsonify(success=True)

    finally:
        # Clean up the temporary file
        if os.path.exists(file_path):
            os.remove(file_path)

def update_image_status(image_id):
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = "UPDATE image SET is_processed = 1 WHERE image_id = %s"
            cursor.execute(query, (image_id,)) 
            connection.commit()
    except mysql.connector.Error as err:
        print(f"Error: {err}")
    finally:
        connection.close()


def insertImageData(image_id,image_data):
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = "INSERT INTO image_data (image_id,image_data) VALUES (%s,%s) "
            cursor.execute(query, (image_id,image_data))  
            connection.commit()
    except mysql.connector.Error as err:
        print(f"Error: {err}")
    finally:
        connection.close()

if __name__ == '__main__':
    app.run(debug=True)
