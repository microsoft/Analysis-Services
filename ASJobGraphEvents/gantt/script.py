from dataclasses import dataclass
from typing import List, Set, Iterable, Optional, cast
import os, sys

from structures import Job, Gantt, Row, new_gantt

import dgml, output


def get_dir(folder: str) -> Set[str]:
    return set(
        [
            os.path.join(folder, filename)
            for filename in os.listdir(folder)
            if os.path.isfile(os.path.join(folder, filename))
        ]
    )


def write_document(content: str, filepath: str) -> None:
    os.makedirs(os.path.dirname(filepath), exist_ok=True)

    with open(filepath, "w") as file:
        file.write(content)


def output_file_path(file: str, out_folder: str) -> str:
    base = os.path.basename(file)
    base, ext = os.path.splitext(base)

    return os.path.join(out_folder, base + ".html")


def make_gantt(file: str, out_folder: str) -> None:
    html = output.html(new_gantt(dgml.read_jobs(file)))

    if not html:
        print(f"No jobs found in {file}; maybe this is not the -annotated file?")
    else:
        write_document(html, output_file_path(file, out_folder))
        print(f'Saving "{output_file_path(file, out_folder)}"')


def make_gantt_dir(folder: str, out_folder: str) -> None:
    for file in get_dir(folder):
        make_gantt(file, out_folder)


# SCRIPT


def print_help() -> None:
    print(
        """
Guide for gantt/script.py

(requires Python 3.8 or later)

Use:

\tpython gantt/script.py <input folder> <output folder>
\t\tRebuilds all graphs in "./data" and writes them to "./output".

\tpython rebuild.py <inputfile> <inputfile> ... <outputfolder>
\t\tRebuilds <inputfile>s and writes them to <outputfolder>
"""
    )


def main() -> None:
    if len(sys.argv) < 3:
        print_help()
    else:
        _, *inputs, output_folder = sys.argv

        for i in inputs:
            if os.path.isfile(i):
                make_gantt(i, output_folder)
            elif os.path.isdir(i):
                make_gantt_dir(i, output_folder)
            else:
                print(f"{i} is not a file or directory.")


if __name__ == "__main__":
    main()
