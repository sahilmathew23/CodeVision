import os
import requests
import json
import re
import sys
import tiktoken  # OpenAI's tokenization library
import google.generativeai as genai  # Gemini API

def call_openai_api(prompt):
    """Call the OpenAI API with the provided prompt and return the response."""
    api_key = os.getenv("OPENAI_API_KEY")
    if not api_key:
        print("Error: OpenAI API key is not set.")
        return None

    url = "https://api.openai.com/v1/chat/completions"
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {api_key}"
    }

    # Token count estimation
    encoder = tiktoken.get_encoding("cl100k_base")
    prompt_tokens = len(encoder.encode(prompt))
    print(f"Number of tokens in the prompt: {prompt_tokens}")

    data = {
        "model": "gpt-4-turbo",
        "messages": [{"role": "user", "content": prompt}]
    }
    
    try:
        response = requests.post(url, headers=headers, json=data)
        if response.status_code == 200:
            raw_response = response.json()
            return raw_response.get("choices", [{}])[0].get("message", {}).get("content", None)
        else:
            print(f"Error with API request: {response.status_code} {response.text}")
            return None
    except Exception as e:
        print(f"Error calling OpenAI API: {e}")
        return None

def call_gemini_api(prompt):
    """Call the Gemini API with the provided prompt and return the response."""
    api_key = os.getenv("GEMINI_API_KEY")
    if not api_key:
        print("Error: Gemini API key is not set.")
        return None

    # Token count estimation
    encoder = tiktoken.get_encoding("cl100k_base")
    prompt_tokens = len(encoder.encode(prompt))
    print(f"Number of tokens in the prompt: {prompt_tokens}")

    genai.configure(api_key=api_key)
    
    try:
        model = genai.GenerativeModel(model_name="gemini-2.0-flash")
        response = model.generate_content(prompt)
        return response.text if response else None
    except Exception as e:
        print(f"Error calling Gemini API: {e}")
        return None
    
def extract_files_from_merged_output(file_path):
    """Extract individual file content from merged_output.txt."""
    with open(file_path, "r", encoding="utf-8") as file:
        content = file.read()

    file_sections = re.split(r"===== (.*?) \((.*?)\) =====", content)[1:]
    extracted_files = {}

    for i in range(0, len(file_sections), 3):
        file_name = file_sections[i].strip()
        file_path = file_sections[i+1].strip()
        file_content = file_sections[i+2].strip()
        extracted_files[file_name] = file_content

    return extracted_files

def enhance(model_name):
    """Enhance the entire project content from merged_output.txt."""
    input_file = "output/merged_output.txt"
    prompt_file = "input/prompt.txt"
    
    print(f"Enhancing project using model: {model_name}")
    
    if not os.path.exists(input_file) or not os.path.exists(prompt_file):
        print("Error: Required files not found.")
        return

    # Read prompt template
    with open(prompt_file, "r", encoding="utf-8") as file:
        prompt_template = file.read()

    # Read entire project content
    with open(input_file, "r", encoding="utf-8") as file:
        project_content = file.read()

    # Prepare prompt with entire project content
    prompt = prompt_template.format(project_content=project_content)

    # Call appropriate API
    if model_name == "gpt-4-turbo":
        output = call_openai_api(prompt)
    elif model_name == "gemini-2.0-flash":
        output = call_gemini_api(prompt)
    else:
        print(f"Error: Unsupported model {model_name}")
        return

    if output:
        # Save enhanced project
        output_file = "output/enhanced_project.txt"
        with open(output_file, "w", encoding="utf-8") as f:
            f.write(output)
        print(f"Enhanced project saved to {output_file}")

        # Extract and save individual files
        output_dir = "/workspaces/CodeVision1/output/enhancedFiles"
        os.makedirs(output_dir, exist_ok=True)
        
        created_files = []
        
        # Add debug print to see the raw output
        print("\nDebug - Raw output:")
        print(output[:500])  # Print first 500 chars
        
        # Split the output into individual files with improved regex
        pattern = r'===== FILE: ([^\n]+?) =====\n```[^\n]*\n(.*?)\n```(?:\n===== END FILE =====)?'
        matches = re.finditer(pattern, output, re.DOTALL)
        
        for match in matches:
            filename = match.group(1).strip()
            content = match.group(2).strip()
            
            print(f"\nDebug - Processing file: {filename}")
            print(f"Content length: {len(content)} characters")
            
            # Remove tmp path prefix if present
            filename = re.sub(r'^/tmp/[^/]+/', '', filename)
            
            # Ensure the filename is sanitized and remove any invalid characters
            safe_filename = os.path.normpath(filename).lstrip(os.sep)
            output_path = os.path.join(output_dir, safe_filename)
            
            print(f"Debug - Writing to: {output_path}")
            
            # Ensure directory exists
            os.makedirs(os.path.dirname(output_path), exist_ok=True)
            created_files.append(output_path)
            
            try:
                with open(output_path, "w", encoding="utf-8") as f:
                    f.write(content)
                print(f"Successfully saved: {output_path}")
            except Exception as e:
                print(f"Error saving file {output_path}: {e}")
        
        # Print summary of created files
        print("\nSummary of created files:")
        for file_path in created_files:
            print(f"- {file_path}")
    else:
        print("Project enhancement failed.")

# Run the enhancement process
if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Error: Model name argument is missing.")
        sys.exit(1)

    model_name = sys.argv[1]
    enhance(model_name)
