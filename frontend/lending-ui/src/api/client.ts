const API_BASE_URL = "http://localhost:5036"; // replace with your actual API port from Step 6

export interface LoanApplicationRequest {
  loanAmount: number;
  assetValue: number;
  creditScore: number;
}

export interface LoanDecisionResponse {
  id: string;
  loanAmount: number;
  assetValue: number;
  creditScore: number;
  loanToValue: number;
  decision: "Approved" | "Declined";
  declineReason: string | null;
  createdAtUtc: string;
}

export interface PortfolioStatisticsResponse {
  totalApplications: number;
  approvedCount: number;
  declinedCount: number;
  totalValueOfLoansWritten: number;
  meanLoanToValue: number;
}

export interface ValidationError {
  errors: Record<string, string[]>;
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const body = await response.json().catch(() => null);
    throw new Error(
      body?.errors
        ? Object.values(body.errors).flat().join(" ")
        : `Request failed with status ${response.status}`
    );
  }
  return response.json();
}

export async function submitApplication(
  request: LoanApplicationRequest
): Promise<LoanDecisionResponse> {
  const response = await fetch(`${API_BASE_URL}/api/applications`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(request),
  });
  return handleResponse<LoanDecisionResponse>(response);
}

export async function getApplications(): Promise<LoanDecisionResponse[]> {
  const response = await fetch(`${API_BASE_URL}/api/applications`);
  return handleResponse<LoanDecisionResponse[]>(response);
}

export async function getStatistics(): Promise<PortfolioStatisticsResponse> {
  const response = await fetch(`${API_BASE_URL}/api/statistics`);
  return handleResponse<PortfolioStatisticsResponse>(response);
}