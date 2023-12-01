#!/bin/bash

# Recursively find and delete 'bin' and 'obj' folders
find . -type d \( -name bin -o -name obj -name .vs \) -exec rm -rf {} +

echo "Cleaned up bin and obj folders."