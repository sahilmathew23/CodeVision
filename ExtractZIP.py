import shutil
import os
import zipfile

# Define source and destination paths
source_dir = "/workspaces/CodeVision1/input"
destination_dir = "/workspaces/CodeVision1/output/ZIP/Extracted"

# Ensure destination directory exists
os.makedirs(destination_dir, exist_ok=True)

# Find the first .zip file in the source directory
zip_files = [f for f in os.listdir(source_dir) if f.endswith(".zip")]

if not zip_files:
    print("No .zip file found in the input directory.")
else:
    zip_file_path = os.path.join(source_dir, zip_files[0])
    
    # Extract the zip file
    extracted_path = os.path.join(destination_dir, os.path.splitext(zip_files[0])[0])
    os.makedirs(extracted_path, exist_ok=True)
    
    with zipfile.ZipFile(zip_file_path, 'r') as zip_ref:
        zip_ref.extractall(extracted_path)
        print(f"Extracted {zip_files[0]} to {extracted_path}")
