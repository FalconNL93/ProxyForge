#!/usr/bin/env bash
set -e

VENV_DIR=".venv"

if [ ! -d "$VENV_DIR" ]; then
  echo "Creating virtual environment in $VENV_DIR"
  python3 -m venv "$VENV_DIR"
else
  echo "Virtual environment already exists at $VENV_DIR"
fi

echo "Activating virtual environment"
source "$VENV_DIR/bin/activate"

echo "Upgrading pip"
pip install --upgrade pip

echo "Installing packages from requirements.txt"
pip install -r requirements.txt

echo "Installed packages:"
pip list

echo "Virtual environment activated. Run your commands now."
$SHELL


