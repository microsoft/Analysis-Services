from dataclasses import dataclass
from typing import List, Set, Iterable, Optional, cast
from datetime import datetime
import xml.etree.ElementTree as ET

from structures import Job
from gantt_types import ThreadId


def read_jobs(filename: str) -> List[Job]:
    jobs: List[Job] = []

    doc = ET.parse(filename)
    root = doc.getroot()

    try:
        nodes = [child for child in root if "nodes" in child.tag.lower()][0]
    except IndexError:
        return jobs

    for node in nodes:
        if job := parse_job_node(node):
            jobs.append(job)

    return jobs


def parse_iso(time: str) -> datetime:
    if time[-1].lower() == "z":
        time = time[:-1]

    return datetime.fromisoformat(time)


def parse_thread_id(s: str) -> ThreadId:
    return ThreadId(int(s))


def strip_newlines(s: str) -> str:

    return "".join([c for c in s if ord(c) > 32])


def parse_job_node(node: ET.Element) -> Optional[Job]:
    for attr, value in node.attrib.items():
        if attr == "StartedAt":
            start = parse_iso(value)
        if attr == "FinishedAt":
            end = parse_iso(value)
        if attr == "Label":
            name = value
        if attr == "Slot" or attr == "Thread":
            thread = value

    try:
        return Job(start, end, strip_newlines(name), parse_thread_id(thread))
    except UnboundLocalError:
        # most likely doesn't include "Thread" or "Slot" attribute.
        return None
