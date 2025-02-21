from flask import Flask, request, render_template, jsonify, redirect, url_for, send_from_directory
import os
import subprocess

app = Flask(__name__)

# Define the directory where the uploaded file will be saved
UPLOAD_FOLDER = '/workspaces/CodeVision1/input'
EXTRACTED_FOLDER = '/workspaces/CodeVision1/output/ZIP/Extracted_files'

if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)
    print(f"Folder '{UPLOAD_FOLDER}' created.")
else:
    print(f"Folder '{UPLOAD_FOLDER}' already exists.")

if not os.path.exists(EXTRACTED_FOLDER):
    os.makedirs(EXTRACTED_FOLDER)
    print(f"Folder '{EXTRACTED_FOLDER}' created.")
else:
    print(f"Folder '{EXTRACTED_FOLDER}' already exists.")

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
                #subprocess.run(["python", "run_pipeline.py", file.filename], check=True)
                
                # Define the path to the extracted zip file
                extracted_file_path = '/workspaces/CodeVision1/output/ZIP'
                
                # Check if the extracted zip file exists
                if os.path.exists(extracted_file_path):
                    print(f"Extracted file ZIP path: {extracted_file_path}/Extracted_files.zip")

                    # Create the downloadable link for the extracted ZIP file
                    download_link = url_for('download_file', filename='Extracted_files.zip', _external=True)
                    return jsonify({"message": "Project Enhancement Completed.", "download_link": download_link}), 200
                else:
                    return jsonify({"message": "Error: Extracted zip file not found after processing."}), 500
            except subprocess.CalledProcessError as e:
                return jsonify({"message": f"Error in running pipeline: {e}"}), 500
        else:
            return jsonify({"message": "Invalid file type. Only .zip files are allowed."}), 400
    return render_template('upload.html')


# Route to serve the extracted .zip file
@app.route('/download/<filename>')
def download_file(filename):
    return send_from_directory(EXTRACTED_FOLDER, filename, as_attachment=True)

if __name__ == '__main__':
    app.run(debug=True)