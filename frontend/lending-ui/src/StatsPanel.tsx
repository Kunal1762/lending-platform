import { useEffect, useState } from "react";
import { getStatistics, type PortfolioStatisticsResponse } from "./api/client";

interface StatsPanelProps {
  refreshKey: number;
}

function StatsPanel({ refreshKey }: StatsPanelProps) {
  const [stats, setStats] = useState<PortfolioStatisticsResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getStatistics()
      .then(setStats)
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load statistics."));
  }, [refreshKey]);

  if (error) return <p className="error">{error}</p>;
  if (!stats) return <p>Loading statistics...</p>;

  return (
    <div className="stats">
      <h2>Portfolio statistics</h2>
      <ul>
        <li>Total applicants: {stats.totalApplications}</li>
        <li>Approved: {stats.approvedCount}</li>
        <li>Declined: {stats.declinedCount}</li>
        <li>Total value of loans written: £{stats.totalValueOfLoansWritten.toLocaleString()}</li>
        <li>Mean LTV across all applications: {stats.meanLoanToValue}%</li>
      </ul>
    </div>
  );
}

export default StatsPanel;