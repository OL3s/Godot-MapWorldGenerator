#!/bin/sh
printf '\033c\033]0;%s\a' Unnamed
base_path="$(dirname "$(realpath "$0")")"
"$base_path/WorldGenerator.x86_64" "$@"
