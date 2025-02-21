import subprocess
import sys

def run_script(script_name, *args):
    """Runs a Python script with optional arguments."""
    try:
        cmd = ["python", script_name] + list(args)
        result = subprocess.run(cmd, check=True)
        print(f"{script_name} executed successfully.")
    except subprocess.CalledProcessError as e:
        print(f"Error running {script_name}: {e}")

def main():
    # Ensure the script is called with the correct number of arguments
    if len(sys.argv) < 2:
        print("Error: Filename argument is missing.")
        return
    
    uploaded_filename = sys.argv[1]
    
    # Prompt or use the uploaded filename as the project name
    project_name = uploaded_filename  # Use the uploaded file name as the project name
    
    # Construct the directory to scan dynamically
    directory_to_scan = f"/workspaces/CodeVision1/{project_name}"
    output_file = "/workspaces/CodeVision1/output/merged_output.txt"
    
    print(f"Scanning project: {project_name}...")
    print("Starting scanAndMerge.py...")
    run_script("scanAndMerge.py", directory_to_scan, output_file)
    
    print("Starting enhance.py...")
    run_script("enhance.py")
    
    print("Pipeline execution complete.")

if __name__ == "__main__":
    main()
