from typing import Tuple, List, NamedTuple, Optional, NewType
from datetime import datetime

import utility
from gantt_types import ThreadId, Millisecond, Second

import operator


class Job(NamedTuple):
    start: datetime
    end: datetime
    name: str
    thread: ThreadId


class Row(NamedTuple):
    jobs: List[Job]
    thread: ThreadId


Gantt = List[Row]


def add_job(row: Row, job: Job) -> None:
    assert row.thread == job.thread, f"row: {row.thread}, job: {job.thread}"

    if row.jobs:
        assert (
            row.jobs[-1].end <= job.start
        ), f"{row.jobs[-1].end} is not less than {job.start} (thread id: {row.thread})"

    row.jobs.append(job)


def new_row(job: Job) -> Row:
    return Row([job], job.thread)


def row_duration_ms(row: Row) -> Millisecond:
    return utility.duration_ms(row.jobs[0].start, row.jobs[-1].end)


def row_computing_duration_ms(row: Row) -> Millisecond:
    return Millisecond(sum([job_duration_ms(job) for job in row.jobs]))


def row_with_thread(g: Gantt, thread: ThreadId) -> Optional[Row]:
    for row in g:
        if row.thread == thread:
            return row
    return None


def add_row(g: Gantt, row: Row) -> None:
    g.append(row)


def new_gantt(jobs: List[Job]) -> Gantt:
    g: Gantt = []

    for job in sorted(jobs, key=operator.attrgetter("start")):
        if row := row_with_thread(g, job.thread):
            add_job(row, job)
        else:
            add_row(g, new_row(job))

    return g


def job_duration_ms(job: Job) -> Millisecond:
    return utility.duration_ms(job.start, job.end)
