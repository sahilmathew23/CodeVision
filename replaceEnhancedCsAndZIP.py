import os
import shutil
import zipfile

def find_interface_implementation_pair(file_name, src_folder):
    """Find interface file for a given implementation file."""
    if not file_name.startswith('I') and file_name.endswith('.cs'):
        interface_name = 'I' + file_name
        interface_path = os.path.join(src_folder, interface_name)
        if os.path.exists(interface_path):
            return interface_path
    return None

def find_files_in_directory(file_name, search_directory):
    """Recursively search for all instances of a file in the given directory and subdirectories."""
    matching_files = []
    for root, _, files in os.walk(search_directory):
        if file_name in files:
            matching_files.append(os.path.join(root, file_name))
    return matching_files

def replace_modified_files(src_folder, dest_folder):
    """Replace files from source folder to matching files in destination folder and its subdirectories."""
    if not os.path.exists(src_folder) or not os.path.exists(dest_folder):
        print("Source or destination folder does not exist")
        return False
    
    replaced_files = 0
    for file_name in os.listdir(src_folder):
        if file_name.endswith(".cs"):
            # Skip interface files as they'll be handled with their implementations
            if file_name.startswith('I'):
                continue
                
            src_file = os.path.join(src_folder, file_name)
            matching_files = find_files_in_directory(file_name, dest_folder)
            
            # Find interface file if it exists
            interface_file = find_interface_implementation_pair(file_name, src_folder)
            
            if matching_files:
                for dest_file in matching_files:
                    # Copy implementation file
                    shutil.copy2(src_file, dest_file)
                    print(f"Replaced: {dest_file}")
                    replaced_files += 1
                    
                    # Copy interface file if it exists
                    if interface_file:
                        dest_interface = os.path.join(os.path.dirname(dest_file), 'I' + file_name)
                        shutil.copy2(interface_file, dest_interface)
                        print(f"Copied interface: {dest_interface}")
                        replaced_files += 1
            else:
                print(f"No matching files found for: {file_name}")
    
    return replaced_files > 0

def zip_directory(directory, zip_path):
    """Create a zip file from a directory, preserving the directory structure."""
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, _, files in os.walk(directory):
            for file in files:
                file_path = os.path.join(root, file)
                arcname = os.path.relpath(file_path, directory)
                zipf.write(file_path, arcname)
    print(f"Created zip file: {zip_path}")

def print_csproj_files(directory):
    """Print all .csproj files found in the given directory and its subdirectories."""
    found = False
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith('.csproj'):
                file_path = os.path.join(root, file)
                print(f"Found .csproj file: {file_path}")
                
                # Read and check OutputType
                try:
                    with open(file_path, 'r', encoding='utf-8') as f:
                        content = f.read()
                        if '<OutputType>Exe</OutputType>' in content:
                            print(f"  Output Type: Executable")
                        elif '<OutputType>Library</OutputType>' in content:
                            print(f"  Output Type: Library")
                        else:
                            print(f"  Output Type: Not specified")
                except Exception as e:
                    print(f"  Error reading file: {str(e)}")
                
                found = True
    if not found:
        print("No .csproj files found in the directory.")

def copy_additional_files(dest_folder):
    """Copy additional required files to the destination folder."""
    additional_files = [
        "/workspaces/CodeVision1/output/enhanced_project.txt",
        "/workspaces/CodeVision1/output/merged_output.txt"
    ]
    
    for file_path in additional_files:
        if os.path.exists(file_path):
            dest_file = os.path.join(dest_folder, os.path.basename(file_path))
            shutil.copy2(file_path, dest_file)
            print(f"Copied additional file: {dest_file}")
            
            # Rename merged_output.txt to raw_project.txt after copying
            if file_path.endswith('merged_output.txt'):
                new_name = os.path.join(dest_folder, 'raw_project.txt')
                os.rename(dest_file, new_name)
                print(f"Renamed {dest_file} to {new_name}")
        else:
            print(f"Warning: Additional file not found: {file_path}")

if __name__ == "__main__":
    # Define paths
    src_folder = "/workspaces/CodeVision1/output/ClassFiles"
    dest_folder = "/workspaces/CodeVision1/output/ZIP/Extracted"
    zip_path = "/workspaces/CodeVision1/output/ZIP/Extracted_files.zip"
    
    # Replace files
    if replace_modified_files(src_folder, dest_folder):
        # Print .csproj files before zipping
        print("\nChecking for .csproj files:")
        print_csproj_files(dest_folder)
        
        # Copy additional files
        print("\nCopying additional files:")
        copy_additional_files(dest_folder)
        
        # Create zip file
        zip_directory(dest_folder, zip_path)
    else:
        print("No files were replaced. Zip creation skipped.")
