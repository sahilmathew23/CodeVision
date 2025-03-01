from flask import Flask, request, render_template, jsonify, redirect, url_for, send_from_directory
import os
import subprocess

app = Flask(__name__)

UPLOAD_FOLDER = '/workspaces/CodeVision1/input'
EXTRACTED_FOLDER = '/workspaces/CodeVision1/output/ZIP/Extracted_files'

os.makedirs(UPLOAD_FOLDER, exist_ok=True)
os.makedirs(EXTRACTED_FOLDER, exist_ok=True)

app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER
app.config['ALLOWED_EXTENSIONS'] = {'zip'}

def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in app.config['ALLOWED_EXTENSIONS']

@app.route('/')
def home():
    return redirect(url_for('upload_file'))

@app.route('/upload', methods=['GET', 'POST'])
def upload_file():
    if request.method == 'POST':
        if 'file' not in request.files:
            return jsonify({"message": "No file part"}), 400
        file = request.files['file']
        model = request.form.get('model')

        if file.filename == '':
            return jsonify({"message": "No selected file"}), 400

        if file and allowed_file(file.filename):
            filename = os.path.join(app.config['UPLOAD_FOLDER'], file.filename)
            file.save(filename)
            #"scanAndMerge.py", directory_to_scan, output_file
            directory_to_scan = f"{filename}"
            output_file = "/workspaces/CodeVision1/output/merged_output.txt"
            subprocess.run(["python", "scanAndMerge.py", directory_to_scan, output_file], check=True)
            return redirect(url_for('index_page', filename=file.filename, model=model))
        else:
            return jsonify({"message": "Invalid file type. Only .zip files are allowed."}), 400
    return render_template('upload.html')

@app.route('/index')
def index_page():
    filename = request.args.get('filename')
    model = request.args.get('model')
    return render_template('index.html', filename=filename, model=model)

@app.route('/enhance')
def enhance_file():
    filename = request.args.get('filename')
    model = request.args.get('model')
    return render_template('enhance.html', filename=filename, model=model)

@app.route('/download/<filename>')
def download_file(filename):
    return send_from_directory(EXTRACTED_FOLDER, filename, as_attachment=True)

@app.route('/get-info', methods=['POST'])
def get_info():
    data = request.get_json()
    query = data.get('query', '')
    model = data.get('model', '')
    
    if not query:
        return jsonify({"message": "No query provided"}), 400

    try:
        result = subprocess.run(["python", "projectQuery.py", query, model, "enhanced"], capture_output=True, text=True, check=True)
        response_message = result.stdout.strip()
    except subprocess.CalledProcessError as e:
        response_message = f"Error processing query: {e}"

    return jsonify({"message": response_message})

@app.route('/get-info-raw', methods=['POST'])
def get_raw_project_info():
    data = request.get_json()
    query = data.get('query', '')
    model = data.get('model', '')
    
    if not query:
        return jsonify({"message": "No query provided"}), 400

    try:
        result = subprocess.run(["python", "projectQuery.py", query, model, "raw"], capture_output=True, text=True, check=True)
        response_message = result.stdout.strip()
    except subprocess.CalledProcessError as e:
        response_message = f"Error processing query: {e}"

    return jsonify({"message": response_message})

@app.route('/enhance-process', methods=['POST'])
def enhance_process():
    filename = request.form['filename']
    model = request.form['model']

    try:
        subprocess.run(["python", "run_pipeline.py", filename, model], check=True)
        extracted_file_path = '/workspaces/CodeVision1/output/ZIP'
        if os.path.exists(os.path.join(extracted_file_path, "Extracted_files.zip")):
            return send_from_directory(extracted_file_path, 'Extracted_files.zip', as_attachment=True)
        else:
            return jsonify({"message": "Extracted zip file not found."}), 500
    except subprocess.CalledProcessError as e:
        return jsonify({"message": f"Error in running pipeline: {e}"}), 500

if __name__ == '__main__':
    app.run(port=5001, debug=True)
