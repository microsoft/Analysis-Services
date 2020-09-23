from datetime import datetime
from gantt_types import Millisecond, Second


def duration_ms(start_time: datetime, end_time: datetime) -> Millisecond:
    duration = end_time - start_time

    return Millisecond((duration.seconds * 1000000 + duration.microseconds) // 1000)


def ms_to_s(m: Millisecond) -> Second:
    return Second(m / 1000)


def s_to_ms(s: Second) -> Millisecond:
    return Millisecond(s * 1000)
