#!/usr/bin/env python3

"""
Rebuilds a DGML file. Requires Python 3.8.
"""

from typing import Dict, List, Tuple, Set, NamedTuple, Optional
import csv, re, os, operator, sys
import xml.etree.ElementTree as ET


maxsize = sys.maxsize
while True:
    try:
        csv.field_size_limit(maxsize)
        break
    except OverflowError:
        maxsize //= 2

# TYPES


class Row(NamedTuple):
    guid: str
    order_marker: int
    textdata: str


# PARSING


def load_file(filename: str) -> List[Row]:
    """
    Returns a list of events, not sorted or filtered.
    """
    _, ext = os.path.splitext(filename)

    if ext == ".csv":
        with open(filename) as file:
            dict_rows = csv.DictReader(file)
            rows = [
                make_row_from_jarvis(
                    row["MessageText"],
                    row["CurrentActivityId"],
                    int(row["Engine_EventSubclass"]),
                )
                for row in dict_rows
            ]

            return [r for r in rows if r]

    elif ext == ".xml":
        tree = ET.parse(filename)
        ns = {"": "http://tempuri.org/TracePersistence.xsd"}

        xml_rows: List[Optional[Row]] = []

        for event in tree.findall(".//Event", ns):
            xml_rows.append(make_row_from_xml(event, ns))

        return [r for r in xml_rows if r]
    else:
        return []


def make_row_from_xml(event: ET.Element, ns: Dict[str, str]) -> Optional[Row]:
    if event.attrib["id"] != "134":
        return None

    textdata = None
    order_marker = None
    guid = None
    subclass = None

    for col in event.findall("Column", ns):
        if col.attrib["id"] == "46" or col.attrib["id"] == "53":
            guid = col.text

        if col.attrib["id"] == "1":
            subclass = col.text

        if col.attrib["id"] == "10" and col.text:
            order_marker = int(col.text)

        if col.attrib["id"] == "42":
            textdata = col.text

    if textdata and order_marker is not None and guid and subclass:
        suffix = "annotated" if subclass == "2" else "plan"
        return Row(f"{guid}-{suffix}", order_marker, textdata)

    return None


def make_row_from_jarvis(
    message_txt: str, activity_id: str, subclass: int
) -> Optional[Row]:
    guid = activity_id + str(subclass) + ("-annotated" if subclass == 2 else "-plan")

    if "graphcorrelationid" in message_txt.lower():
        match = re.match(
            r"TextData: (.*); GraphCorrelationID: (.*); IntegerData: (.\d*)",
            message_txt,
        )
        if match:
            textdata, order_marker_str = match.group(1, 3)
    else:
        match = re.match(r"TextData: (.*); IntegerData: (.\d*)", message_txt)

        if match:
            textdata, order_marker_str = match.group(1, 2)

    try:
        order_marker = int(order_marker_str)
        return Row(guid, order_marker, textdata)
    except UnboundLocalError:
        return None


def extract_metadata(header_row: Row) -> Optional[Tuple[int, int]]:
    # should really extract things correctly here
    m = re.match(
        r".*Length=\"(\d*)\".*AdditionalEvents=\"(\d*)\".*", header_row.textdata
    )

    if not m:
        return None

    return int(m.group(1)), int(m.group(2))


def remove_pii_tags(protected_data: str) -> str:
    if protected_data[:5] == "<pii>" and protected_data[-6:] == "</pii>":
        return protected_data[5:-6]
    return protected_data


def get_all_guids(data: List[Row]) -> Set[str]:
    return {row.guid for row in data}


# GRAPH


def get_graph(data: List[Row], guid: str) -> Tuple[str, str]:
    rows = [row for row in data if row.guid == guid]

    rows = sorted(rows, key=operator.attrgetter("order_marker"))

    header, *graph_data = rows

    metadata = extract_metadata(header)

    if metadata:
        size, additional_events = metadata
        assert additional_events == len(
            graph_data
        ), f"metadata says there are {additional_events} rows; but there are {len(graph_data)}"

    graph_str_builder = [remove_pii_tags(row.textdata) for row in graph_data]

    return "".join(graph_str_builder), guid


# INPUT/OUTPUT FILES


def get_all_event_files() -> List[str]:
    return [os.path.join("data", f) for f in os.listdir("data")]


def get_output_file(input_file: str, guid: str, output_folder: str) -> str:
    _, input_file = os.path.split(input_file)
    name, ext = os.path.splitext(input_file)

    os.makedirs(output_folder, exist_ok=True)

    return os.path.join(output_folder, f"{name}-{guid}.DGML")


def writefile(filename: str, data: str) -> None:
    with open(filename, "w") as file:
        file.write(data)


def reassemble_file(filename: str) -> List[Tuple[str, str]]:
    result: List[Tuple[str, str]] = []

    try:
        data = load_file(filename)
        guids = get_all_guids(data)

        for guid in guids:
            result.append(get_graph(data, guid))
    except (IndexError, ValueError) as e:
        print(f"error processing {filename}: {e}")

    return result


def all_files() -> None:
    if not os.path.isdir("data"):
        print("directory 'data' does not exist.")
        return

    for input_file in get_all_event_files():
        try:
            data = load_file(input_file)
            guids = get_all_guids(data)

            os.makedirs("output", exist_ok=True)

            for guid in guids:
                graph, _ = get_graph(data, guid)
                output_file = get_output_file(input_file, guid, "output")
                print(f'Saving "{output_file}"')
                writefile(output_file, graph)

        except (IndexError, ValueError) as e:
            print(f"error processing {input_file}: {e}")


# SCRIPT


def print_help() -> None:
    print(
        """
Guide for rebuild.py

(requires Python 3.8 or later)

Use:

\tpython rebuild.py                           \tRebuilds all graphs in "./data" and writes them to "./output".

\tpython rebuild.py <inputfile> <outputfolder>\tRebuilds <inputfile> and writes them to <outputfolder>
"""
    )


def main() -> None:
    if len(sys.argv) == 1:
        print("Reassembling all graphs in ./data")
        all_files()
    if len(sys.argv) == 2:
        print_help()
    if len(sys.argv) == 3:
        _, input_file, output_folder = sys.argv

        for graph, guid in reassemble_file(input_file):
            output_file = get_output_file(input_file, guid, output_folder)
            print(f'Saving "{output_file}"')
            writefile(get_output_file(input_file, guid, output_folder), graph)


if __name__ == "__main__":
    main()
