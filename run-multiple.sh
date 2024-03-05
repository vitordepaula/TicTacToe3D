#Select default pane. Probably an unnecessary line of code
byobu select-pane -t 0

#Split pane 0 into two vertically stacked panes
byobu split-window -v

#Select the newly created pane 1. Again, probably unnecessary as the new pane gets selected after a split
byobu select-pane -t 1

#Split pane 1 horizontally to create two side-by-side panes
byobu split-window -h

#Repeat the selection and splitting process with the top half
byobu select-pane -t 0
byobu split-window -h
#At this point, four equally sized panes have been created.

#Send commands to each pane
for pane in 0 1 2 3; do
  byobu select-pane -t $pane
  byobu send-keys "clear; mono ttt3d.exe" Enter
done
