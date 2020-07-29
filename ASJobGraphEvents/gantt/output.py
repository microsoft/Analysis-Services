from datetime import datetime
from structures import Gantt, Row, Job
from gantt_types import Second, Millisecond
import structures
import utility
import operator

COLORS = [
    "#d50000",
    "#00bfa5",
    "#ff6f00",
    "#aa00ff",
    "#006064",
    "#ffd600",
    "#64dd17",
]

HEADER_COLUMN_WIDTH = 240


def ms_to_px(ms: Millisecond) -> float:
    return ms / 10


def job_to_html(job: Job, start: datetime, color: str) -> str:
    left = ms_to_px(utility.duration_ms(start, job.start)) + HEADER_COLUMN_WIDTH
    width = ms_to_px(structures.job_duration_ms(job))

    return f"""<span class="job" data-descr="{job.name}{chr(10)}Duration: {utility.ms_to_s(structures.job_duration_ms(job)):.2f}s" style="left: {left}px; width: {width}px; background-color: {color}"></span>"""


def row_to_html(
    row: Row, start: datetime, process_num: int, color: str, width: float
) -> str:
    legend_html = f"""<span class="legend" style="width: {HEADER_COLUMN_WIDTH}px">Concurrency Slot {process_num} ({utility.ms_to_s(structures.row_computing_duration_ms(row)):.1f}s)</span>"""

    jobs_html = "\n".join([job_to_html(job, start, color) for job in row.jobs])

    return (
        f"""<div class="row" style="width: {width}px;">{legend_html}{jobs_html}</div>"""
    )


def rownum_to_top(num: int) -> float:
    return num * 2


def make_axis_span(left: float, s: Second) -> str:
    return f"""<span class="axis-tick" style="left: {left}px;">{s} sec</span>"""


def make_axis_html(max_seconds: Second) -> str:
    seconds = [Second(i * 2) for i in range(1000)]

    seconds = [i for i in seconds if i < max_seconds]

    axis_spans = "".join(
        [
            make_axis_span(ms_to_px(utility.s_to_ms(s)) + HEADER_COLUMN_WIDTH, s)
            for s in seconds
        ]
    )

    return f"""<div class="row axis">
    <span class="legend" style="width: {HEADER_COLUMN_WIDTH}px">Total Processing Time</span>
    {axis_spans}
</div>"""


def gantt_to_html(g: Gantt) -> str:
    if not g:
        return ""

    start = min([row.jobs[0].start for row in g])

    max_seconds = max([utility.ms_to_s(structures.row_duration_ms(row)) for row in g])

    rows_html = "\n".join(
        [
            row_to_html(
                row,
                start,
                num + 1,
                COLORS[num % len(COLORS)],
                ms_to_px(utility.s_to_ms(max_seconds)) + HEADER_COLUMN_WIDTH,
            )
            for num, row in enumerate(
                sorted(
                    g,
                    reverse=True,
                    key=lambda r: structures.row_computing_duration_ms(r),
                )
            )
        ]
    )

    return f"""<div class="gantt">{make_axis_html(max_seconds)}{rows_html}</div>"""


def style() -> str:
    with open("./gantt/output.css") as css:
        return f"""<style>{css.read()}</style>"""


def html(g: Gantt) -> str:
    html = f"""
<html>
<head></head>
<body>
<main>
<h1>Gantt Chart</h1>
<p>Max parallelism: {len(g)}</p>
{gantt_to_html(g)}
<h1>Explanation</h1>
<p>
    <ul>
    <li>Each row represents a parallelism "slot"; if "maxParallelism" was 4, then there are 4 rows.</li>
    <li>Each colored block is a job; hover with a mouse to show the name and how long it took.</li>
    <li>Each row shows the total time spent doing jobs to highlight bottlenecks.</li>
    </ul>
</p>
</main>
{style()}
</body>
</html>
"""

    return html if g else ""
