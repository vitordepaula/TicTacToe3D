#!/usr/bin/env bash

# Check byobu available
byobu --version 2>/dev/null || { echo "byobu not available"; exit 1; }

# Delete any extra panes from current window
byobu kill-pane -a

# Split pane 0 into two vertically stacked panes
byobu split-window -v

# Select the newly created pane 1. Again, probably unnecessary as the new pane gets selected after a split
byobu select-pane -t 1

# Split pane 1 horizontally to create two side-by-side panes
byobu split-window -h

# Repeat the selection and splitting process with the top half
byobu select-pane -t 0
byobu split-window -h
# At this point, four equally sized panes have been created.

# Send commands to each pane
for pane in 0 1 2 3; do
  byobu select-pane -t $pane
  byobu send-keys "clear; mono ttt3d.exe" Enter
done
