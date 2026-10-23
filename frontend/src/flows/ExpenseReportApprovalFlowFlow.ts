// Auto-generated flow: ExpenseReport Approval Flow
// Auto-generated approval flow for ExpenseReport. Customize email templates and add conditions as needed.
// Resource: ExpenseReport
// Enabled: true
//
// Nodes:
  // trigger: On ExpenseReport Submit
  // condition: Status = PendingManagerApproval?
  // approval: ExpenseReport Approval
  // action: Send Approval Email
  // trigger: On ExpenseReport Approved
  // action: Send Completion Email
//
// Edges:
  // On ExpenseReport Submit → Status = PendingManagerApproval?
  // Status = PendingManagerApproval? → ExpenseReport Approval (true)
  // ExpenseReport Approval → Send Approval Email
  // On ExpenseReport Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_EXPENSEREPORT_APPROVAL_FLOW_ID = 'flow-ExpenseReport-approval';
