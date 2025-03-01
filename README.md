CodeVision : "See the bigger picture in every line of code" 

Overview

CodeVision is a Python-based tool designed to extract, enhance, and manage C# source code from ZIP archives. It automates the process of extracting C# files, cleaning them, improving their structure, and repackaging them for further use. The tool leverages Google Gemini AI and OpenAI for code enhancement, ensuring high-quality output.

Features

Extracts C# code files from ZIP archives

Enhances code using AI models (Gemini & OpenAI)

Generates each file based on the context of the complete project

Provides a chat feature to discuss both the original and enhanced project

Allows downloading chat output in a .txt file

Supports automated execution via a pipeline

Installation

Prerequisites

Ensure you have the following installed:

Python 3.x

Required dependencies (install using pip)

Setup

Clone the repository:

git clone https://github.com/your-repo/CodeVision.git
cd CodeVision

Install dependencies:

pip install -r requirements.txt

Usage

Running the Application

The main entry point for the project is app.py. Run the following command to start the application:

python app.py

Running the Pipeline

To execute the full process manually, run:

python run_pipeline.py

Running Individual Scripts

Extract ZIP files:

python ExtractZIP.py

Extract C# code:

python extractCSharpCode.py

Enhance the code using AI:

python enhance.py

Replace and repackage:

python replaceEnhancedCsAndZIP.py

Chat about the project:

python projectQuery.py

Workflow

ExtractZIP.py: Unpacks ZIP archives containing C# projects.

extractCSharpCode.py: Scans and extracts .cs files.

cleanUP.py: Removes unnecessary files and cleans the structure.

enhance.py: Uses Gemini & OpenAI to improve the C# code (formatting, comments, etc.).

scanAndMerge.py: Merges multiple files if needed.

replaceEnhancedCsAndZIP.py: Replaces enhanced files and compresses them back into ZIP.

projectQuery.py: Provides a chat interface for discussing the project.

run_pipeline.py: Automates the entire process.

app.py: Main entry point that orchestrates the workflow.