from flask import Flask, request, render_template, jsonify, redirect, url_for, send_from_directory, send_file
import os
import subprocess

app = Flask(__name__)

UPLOAD_FOLDER = '/workspaces/CodeVision1/input'
EXTRACTED_FOLDER = '/workspaces/CodeVision1/output/ZIP'

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
            subprocess.run(["python", "ExtractZIP.py"], check=True)
            subprocess.run(["python", "cs_method_scanner.py"], check=True)
            
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
    EXTRACTED_FOLDER = "/workspaces/CodeVision1/output/ZIP"
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
    try:
        data = request.get_json()  # Get JSON data from the request
        filename = data.get('filename')
        model = data.get('model')

        if not filename or not model:
            return jsonify({"message": "Missing filename or model"}), 400

        # Run the enhancement process
        subprocess.run(["python", "run_pipeline.py", filename, model], check=True)
        
        zip_file_path = '/workspaces/CodeVision1/output/ZIP/Extracted_files.zip'

        if os.path.exists(zip_file_path):
            return send_file(zip_file_path, as_attachment=True)  # Auto-download the file
        else:
            return jsonify({"message": "Extracted zip file not found."}), 500

    except subprocess.CalledProcessError as e:
        return jsonify({"message": f"Error in running pipeline: {e}"}), 500

@app.route('/analyze-structure', methods=['POST'])
@app.route('/refactai', methods=['POST'])
def analyze_code():
    try:
        data = request.get_json()
        filename = data.get('filename')
        target_name = data.get('target_name')
        target_type = data.get('target_type', 'method')  # 'method' or 'class'
        
        if not filename or not target_name:
            return jsonify({"message": "Missing filename or target name"}), 400

        # Determine which mode to use based on the endpoint
        is_refact = request.endpoint == 'refactai'
        command = ["python", "core.py", 
                  "--target", target_name,
                  "--type", target_type]
        
        if is_refact:
            command.append("--refact")

        # Run the core.py script
        result = subprocess.run(command, capture_output=True, text=True, check=True)
        
        # For structure analysis, include graph info
        message = result.stdout
        if not is_refact and os.path.exists("dependencies_graph.png"):
            message += "\n\nCheck the dependencies graph in the output folder."
        
        # Return the analysis results
        return jsonify({"message": message})

    except subprocess.CalledProcessError as e:
        error_type = "RefactAI" if is_refact else "Analysis"
        return jsonify({"message": f"Error in {error_type}: {e.stderr}"}), 500

@app.route('/get-methods')
def get_methods():
    try:
        # Run the cs_method_scanner and capture its output
        result = subprocess.run(["python", "cs_method_scanner.py"], 
                              capture_output=True, 
                              text=True, 
                              check=True)
        
        # Split the output into individual methods
        methods = [method.strip() for method in result.stdout.split('\n') if method.strip()]
        
        return jsonify({"methods": methods})
    except subprocess.CalledProcessError as e:
        return jsonify({"message": f"Error getting methods: {e.stderr}"}), 500

if __name__ == '__main__':
    app.run(port=5001, debug=True)
