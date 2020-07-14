# Author-Profiling

This software is an implementation of Author Profiling Model in 4 languages. 1. English 2. Arabic 3. Portugese 4. Spanish

Authors are profiled on the basis of Gender and Region.

The Model is trained over PAN 2107 provided Twitter data of various users. The training configurtions are kept in StartupPath/Configs directory for each language.

Various text patterns are kept into account for building the model like the language textual words used to express anger, happiness, tech, confusion, love etc. that one can find in StartupPath/<language dir>

Currently for size concerns 20 data files from each language is included into pan folder for test and run purposes.

One can find complete details of model at http://ceur-ws.org/Vol-1866/paper_52.pdf

Contribution guidelines
The Model is implemented in C# and compiled with Visual Studio 2010.

The default path for processing files is StartupPath/pan/<language dir> The default path for processed results is StartupPath/Results/<language dir>

Users can also provide input path with -i switch and output path with -o switch

The output is in Json format

This software was presented to take part in CLEF PAN 2017 contest and is uploaded as is.

Anyone can use/improve/change it for academic and research purposes.

Who do I talk to?
for queries j_ahmadkhan@yahoo.co
