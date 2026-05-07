#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
SVG to PNG Converter - Fixed DLL Path
Manually sets GTK Runtime DLL path
"""

import os
import sys
from pathlib import Path

# GTK Runtime installation path (found automatically)
GTK_PATH = "C:/Program Files/GTK3-Runtime Win64/bin"

# Check if path exists
if not os.path.exists(GTK_PATH):
    print("[ERROR] GTK Runtime not found at:", GTK_PATH)
    print("\nPlease check your installation:")
    print("1. Open: C:/Program Files/")
    print("2. Look for: GTK3-Runtime Win64/")
    print("3. Find: bin/ folder")
    print("\nOr manually specify path in this script (GTK_PATH variable)")
    sys.exit(1)

print("[OK] GTK Runtime found at:", GTK_PATH)

# Add DLL path to Python (Windows 10+ method)
try:
    if sys.version_info >= (3, 8):
        # Modern method for Python 3.8+
        os.add_dll_directory(GTK_PATH)
        print("[OK] Added to DLL directory")
    
    # Also add to PATH environment variable
    current_path = os.environ.get('PATH', '')
    if GTK_PATH not in current_path:
        os.environ['PATH'] = GTK_PATH + ';' + current_path
        print("[OK] Added to PATH environment")
    
except Exception as e:
    print("[ERROR] Failed to add DLL path:", str(e))
    sys.exit(1)

# Test if cairo is accessible
print("\n[TEST] Checking Cairo library...")
try:
    import ctypes
    cairo_dll_path = os.path.join(GTK_PATH, "libcairo-2.dll")
    cairo = ctypes.CDLL(cairo_dll_path)
    print("[OK] Cairo DLL loaded successfully:", cairo_dll_path)
except Exception as e:
    print("[ERROR] Cairo DLL test failed:", str(e))
    print("\nPossible solutions:")
    print("1. Check if libcairo-2.dll exists in:", GTK_PATH)
    print("2. Try restarting terminal")
    print("3. Use online converter: https://cloudconvert.com/svg-to-png")
    sys.exit(1)

# Now import conversion libraries
print("\n[LOAD] Importing SVG libraries...")
try:
    from svglib.svglib import svg2rlg
    from reportlab.graphics import renderPM
    print("[OK] SVG libraries loaded successfully")
except Exception as e:
    print("[ERROR] Failed to import libraries:", str(e))
    sys.exit(1)

# Paths - SVG source and PNG output
svg_dir = Path("Assets/Sprites/SVG")
png_dir = Path("Assets/Sprites")  # PNG outputs to parent directory

# List all SVG files in SVG directory
svg_files = [f.name for f in svg_dir.glob("*.svg")]

if not svg_files:
    print("[WARN] No SVG files found in:", svg_dir)
    sys.exit(0)

print("[INFO] Found", len(svg_files), "SVG files")
print("\n[CONVERT] Starting SVG to PNG conversion...\n")

converted_count = 0
for svg_file in svg_files:
    svg_path = svg_dir / svg_file
    png_file = svg_file.replace(".svg", ".png")
    png_path = png_dir / png_file  # PNG saves to Assets/Sprites/ (parent directory)
    
    print("[PROCESS]", svg_file, "...")
    
    if svg_path.exists():
        try:
            # Load SVG
            drawing = svg2rlg(str(svg_path))
            
            if drawing:
                # Save as PNG
                renderPM.drawToFile(drawing, str(png_path), fmt='PNG')
                print("[SUCCESS]", svg_file, "->", png_file)
                converted_count += 1
            else:
                print("[WARN] Could not parse:", svg_file)
        except Exception as e:
            print("[ERROR]", svg_file, "-", str(e))
    else:
        print("[WARN] File not found:", svg_file)

print(f"\n[COMPLETE] {converted_count}/{len(svg_files)} files converted")
print("\n[OUTPUT] PNG files location:", png_dir)
print("\n[NEXT STEPS]")
print("1. Open Unity Editor")
print("2. Find PNG files in: Assets/Sprites/")
print("3. Set Pixels Per Unit = 32")
print("4. Update Player SpriteRenderer.sprite")
print("\n[DONE] All set! Your sprites are ready.")