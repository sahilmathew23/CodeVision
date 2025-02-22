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

def count_tokens(prompt):
    """Estimate the number of tokens in the prompt."""
    return len(prompt.split())

def call_gemini_api(prompt):
    """Call the Gemini API with the provided prompt and return the response."""
    api_key = os.getenv("GEMINI_API_KEY")
    if not api_key:
        print("Error: Gemini API key is not set.")
        return None

    # Count tokens in the prompt
    token_count = count_tokens(prompt)
    print(f"Number of tokens in the prompt: {token_count}")

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
    """Enhance each extracted file from merged_output.txt."""
    input_file = "output/merged_output.txt"
    prompt_file = "input/prompt.txt"
    
    print(f"Enhancing code using model: {model_name}")
    
    if not os.path.exists(input_file):
        print("Error: Input file not found.")
        return
    if not os.path.exists(prompt_file):
        print("Error: Prompt file not found.")
        return

    # Read prompt template
    with open(prompt_file, "r", encoding="utf-8") as file:
        prompt_template = file.read()

    extracted_files = extract_files_from_merged_output(input_file)
    output_dir = "output/enhancedClassFiles"
    os.makedirs(output_dir, exist_ok=True)

    for file_name, file_content in extracted_files.items():
        prompt = prompt_template.format(file_name=file_name, file_content_str=file_content)

        if model_name == "gpt-4-turbo":
            output = call_openai_api(prompt)
        elif model_name == "gemini-2.0-flash":
            output = call_gemini_api(prompt)
        else:
            print(f"Error: Unsupported model {model_name}")
            return

        if output:
            output_file = os.path.join(output_dir, f"enhanced_{file_name}")
            with open(output_file, "w", encoding="utf-8") as f:
                f.write(output)
            print(f"Enhanced version of {file_name} saved to {output_file}\n")
        else:
            print(f"Enhancement failed for {file_name}.")

    input_dir = "/workspaces/CodeVision1/output/enhancedClassFiles"
    output_file = "/workspaces/CodeVision1/output/enhanced_merged_output.txt"

    separator = "\n" + "=" * 80 + "\n"  # A clear separator line

    with open(output_file, "w", encoding="utf-8") as outfile:
        for filename in sorted(os.listdir(input_dir)):  # Sorting for order
            file_path = os.path.join(input_dir, filename)
            if os.path.isfile(file_path):  # Ensure it's a file
                with open(file_path, "r", encoding="utf-8") as infile:
                    outfile.write(f"\nFile: {filename}\n")  # File name header
                    outfile.write(separator)  # Separator line
                    outfile.write(infile.read().strip() + "\n")  # File content
                    outfile.write(separator)  # Another separator for next file

    print(f"All files merged into {output_file}")
# Run the enhancement process
if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Error: Model name argument is missing.")
        sys.exit(1)

    model_name = sys.argv[1]
    enhance(model_name)
