"Assumptions"

## Overlapping LTV bands (sub-£1m loans)

the ltv thrisholds given are
    If LTV < 60% → credit score ≥ 750
    If LTV < 80% → credit score ≥ 800
    If LTV < 90% → credit score ≥ 900
    If LTV ≥ 90% → decline

an LTV of 50% satisfies all three
conditions simultaneously. so i check them in order and the one satisfies first
makes the decision

    LTV < 60%          750
    60% ≤ LTV < 80%    800
    80% ≤ LTV < 90%    900
    LTV ≥ 90%          declined

This is the only interpretation that produces a single, unambiguous required
score for every possible LTV value. 

## Inclusive vs exclusive boundary at 60%

For loans ≥ £1m, the brief says LTV "must be 60% or less" (inclusive — 60%
itself is fine). For loans < £1m, the first band is "LTV < 60%" (exclusive —
60% itself falls into the next band, requiring 800 not 750). This
inconsistency is implemented literally as written rather than "corrected,"
since I have no way to confirm which was intended. Would confirm with the
business before shipping.
