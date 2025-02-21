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
    directory_to_scan = f"/workspaces/CodeVision1/input/{project_name}"
    output_file = "/workspaces/CodeVision1/output/merged_output.txt"
    
    #this will combine the input in 1 file
    print(f"Scanning project: {project_name}...")
    print("Starting scanAndMerge.py...\n")
    run_script("scanAndMerge.py", directory_to_scan, output_file)
    print()
    print()
    
    #this will call the model and enhance the code
    print("Starting enhance.py...\n")
    run_script("enhance.py")
    print()
    print()

    #this will clean file & get required code and save it in Class files
    print("Starting extractCSharpCode.py...\n")
    run_script("extractCSharpCode.py")
    print()
    print()
    
    #this will extract .zip file that was uploaded in ZIP/Extracted folder
    print("Starting ExtractZIP.py...\n")
    run_script("ExtractZIP.py")
    print()
    print()
    
    #this will replace ClassFiles in the Extracted Folder & ZIP it
    print("Starting replaceEnhancedCsAndZIP.py...\n")
    run_script("replaceEnhancedCsAndZIP.py")
    print()
    print()
    
    #Removes temp files & folders
    print("Starting cleanUp.py...\n")
    run_script("cleanUP.py")
    print()
    print()
    
    print("Pipeline execution complete.\n")

if __name__ == "__main__":
    main()
