import { useState } from "react";
import { submitApplication, type LoanDecisionResponse } from "./api/client";
import StatsPanel from "./StatsPanel";
import "./App.css";

function App() {
  const [loanAmount, setLoanAmount] = useState("");
  const [assetValue, setAssetValue] = useState("");
  const [creditScore, setCreditScore] = useState("");
  const [refreshKey, setRefreshKey] = useState(0);
  const [result, setResult] = useState<LoanDecisionResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const liveLtv =
    Number(loanAmount) > 0 && Number(assetValue) > 0
      ? ((Number(loanAmount) / Number(assetValue)) * 100).toFixed(2)
      : null;

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setResult(null);
    setIsSubmitting(true);

    try {
      const response = await submitApplication({
        loanAmount: Number(loanAmount),
        assetValue: Number(assetValue),
        creditScore: Number(creditScore),
      });
      setResult(response);
      setRefreshKey((k) => k + 1);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Something went wrong.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="app">
      <h1>Loan Application</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Loan amount (GBP)
          <input
            type="number"
            value={loanAmount}
            onChange={(e) => setLoanAmount(e.target.value)}
            required
            min="0"
          />
        </label>

        <label>
          Asset value (GBP)
          <input
            type="number"
            value={assetValue}
            onChange={(e) => setAssetValue(e.target.value)}
            required
            min="0"
          />
        </label>

        <label>
          Credit score (1–999)
          <input
            type="number"
            value={creditScore}
            onChange={(e) => setCreditScore(e.target.value)}
            required
            min="1"
            max="999"
          />
        </label>

        {liveLtv && <p className="ltv-preview">Estimated LTV: {liveLtv}%</p>}

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Submitting..." : "Submit application"}
        </button>
      </form>

      {error && <p className="error">{error}</p>}

      {result && (
        <div className={`result ${result.decision.toLowerCase()}`}>
          <h2>{result.decision}</h2>
          <p>LTV: {result.loanToValue}%</p>
          {result.declineReason && <p>{result.declineReason}</p>}
        </div>
      )}
      <StatsPanel refreshKey={refreshKey} />
    </div>
    
  );
}

export default App;