#!/bin/sh
echo -ne '\033c\033]0;240614_project_extraction\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/program.x86_64" "$@"
