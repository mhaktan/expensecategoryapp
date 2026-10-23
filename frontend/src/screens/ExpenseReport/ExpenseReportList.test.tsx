import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import { ExpenseReportList } from './ExpenseReportList';

// Mock dataProvider
vi.mock('../../dataProvider', () => ({
  dataProvider: {
    list: vi.fn().mockResolvedValue({ data: [], total: 0 }),
    create: vi.fn().mockResolvedValue({}),
    update: vi.fn().mockResolvedValue({}),
    delete: vi.fn().mockResolvedValue(undefined),
  },
  isAuthenticated: () => true,
  getAuthToken: () => 'test-token',
}));



const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: false } },
});

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <MemoryRouter>
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  </MemoryRouter>
);

describe('ExpenseReportList', () => {
  it('should render without crashing', () => {
    const { container } = render(<ExpenseReportList />, { wrapper });
    expect(container).toBeDefined();
  });

  it('should show title', () => {
    render(<ExpenseReportList />, { wrapper });
    expect(screen.getByText('ExpenseReport')).toBeDefined();
  });

  it('should have create button', () => {
    render(<ExpenseReportList />, { wrapper });
    expect(screen.getByText('+ Create ExpenseReport')).toBeDefined();
  });

});
