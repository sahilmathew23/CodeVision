import os
import requests
import json
import re
import tiktoken  # OpenAI's tokenization library

def call_openai_api(prompt):
    """Call the OpenAI API with the provided prompt and return the response."""
    api_key = os.getenv("OPENAI_API_KEY")
    url = 'https://api.openai.com/v1/chat/completions'
    headers = {
        'Content-Type': 'application/json',
        'Authorization': f'Bearer {api_key}'
    }

    encoder = tiktoken.get_encoding("cl100k_base")
    prompt_tokens = len(encoder.encode(prompt))
    print(f"Number of tokens in the prompt: {prompt_tokens}")

    data = {
        'model': 'gpt-4-turbo',
        'messages': [{'role': 'user', 'content': prompt}]
    }
    try:
        response = requests.post(url, headers=headers, data=json.dumps(data))
        if response.status_code == 200:
            raw_response = response.json()
            return raw_response['choices'][0]['message']['content'] if 'choices' in raw_response else None
        else:
            print(f"Error with API request: {response.status_code} {response.text}")
            return None
    except Exception as e:
        print(f"Error calling OpenAI API: {e}")
        return None

def extract_files_from_merged_output(file_path):
    """Extract individual file content from merged_output.txt."""
    with open(file_path, "r", encoding="utf-8") as file:
        content = file.read()
    
    file_sections = re.split(r'===== (.*?) \((.*?)\) =====', content)[1:]
    extracted_files = {}
    
    for i in range(0, len(file_sections), 3):
        file_name = file_sections[i].strip()
        file_path = file_sections[i+1].strip()
        file_content = file_sections[i+2].strip()
        extracted_files[file_name] = file_content
    
    return extracted_files

def enhance():
    """Enhance each extracted file from merged_output.txt."""
    input_file = "output/merged_output.txt"
    prompt_file = "input/prompt.txt"
    
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
    
    for file_name, file_content in extracted_files.items():
        prompt = prompt_template.format(file_name=file_name, file_content_str=file_content)
        output = call_openai_api(prompt)
        
        if output:
            output_file = f"output/enhanced_{file_name}"
            with open(output_file, "w", encoding="utf-8") as f:
                f.write(output)
            print(f"Enhanced version of {file_name} saved to {output_file}")
        else:
            print(f"Enhancement failed for {file_name}.")

# Run the enhancement process
enhance()
