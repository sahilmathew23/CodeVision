from flask import Flask, request, render_template, jsonify, redirect, url_for
import os
import subprocess

app = Flask(__name__)

# Define the directory where the uploaded file will be saved
UPLOAD_FOLDER = '/workspaces/CodeVision1/'
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER
app.config['ALLOWED_EXTENSIONS'] = {'zip'}

# Function to check allowed file extension
def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in app.config['ALLOWED_EXTENSIONS']

# Redirect the root URL to the /upload page
@app.route('/')
def home():
    return redirect(url_for('upload_file'))

# Route to upload the file
@app.route('/upload', methods=['GET', 'POST'])
def upload_file():
    if request.method == 'POST':
        # Check if the request has the file part
        if 'file' not in request.files:
            return jsonify({"message": "No file part"}), 400
        file = request.files['file']
        
        # If no file is selected
        if file.filename == '':
            return jsonify({"message": "No selected file"}), 400
        
        # If file is valid
        if file and allowed_file(file.filename):
            filename = os.path.join(app.config['UPLOAD_FOLDER'], file.filename)
            file.save(filename)
            
            # Automatically run run_pipeline.py with the uploaded file name as argument
            try:
                subprocess.run(["python", "run_pipeline.py", file.filename], check=True)
                return jsonify({"message": f"Project Enhancement Completed..."}), 200
            except subprocess.CalledProcessError as e:
                return jsonify({"message": f"Error in running pipeline: {e}"}), 500
        else:
            return jsonify({"message": "Invalid file type. Only .zip files are allowed."}), 400
    return render_template('upload.html')

if __name__ == '__main__':
    app.run(debug=True)
