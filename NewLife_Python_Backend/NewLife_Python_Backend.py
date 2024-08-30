from flask import Flask, request, jsonify
import cv2
import numpy as np
import json
import os
import tempfile

app = Flask(__name__)

@app.route('/process_image', methods=['POST'])
def process_image():
    # Check if the request contains an image file
    if 'image' not in request.files:
        return jsonify({"error": "No image uploaded"}), 400
    
    # Get the image file from the request
    file = request.files['image']
    
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

        # Return the serialized data as a JSON response
        return jsonify({"contours": contours_json})

    finally:
        # Clean up the temporary file
        if os.path.exists(file_path):
            os.remove(file_path)

if __name__ == '__main__':
    app.run(debug=True)
