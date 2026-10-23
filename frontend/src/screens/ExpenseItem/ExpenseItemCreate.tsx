import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type ExpenseItemRecord = {
  id: string | number;
  description: string;
  expenseDate: string;
  amount: number;
  currency: string;
  exchangeRate?: number;
  amountInTRY?: number;
  receiptNo?: string;
  expenseReportId: string;
  expenseCategoryId: string;
};

interface ExpenseItemCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const ExpenseItemCreate: React.FC<ExpenseItemCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<ExpenseItemRecord>>({});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<ExpenseItemRecord>) => dataProvider.create('ExpenseItem', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'ExpenseItem', values as Record<string, unknown>); onSuccess(); onClose(); setForm({}); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Create ExpenseItem</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Description *" value={String(form.description ?? '')} onChange={(v) => setField('description', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Expense Date *" value={String(form.expenseDate ?? '')} onChange={(v) => setField('expenseDate', v)} withTime />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Amount *" value={String(form.amount ?? '')} onChange={(v) => setField('amount', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Currency *" value={String(form.currency ?? '')} onChange={(v) => setField('currency', v ? Number(v) : null)} searchable={false} options={[{ label: "TRY", value: "0" }, { label: "USD", value: "1" }, { label: "EUR", value: "2" }, { label: "GBP", value: "3" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Exchange Rate" value={String(form.exchangeRate ?? '')} onChange={(v) => setField('exchangeRate', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Amount In T R Y" value={String(form.amountInTRY ?? '')} onChange={(v) => setField('amountInTRY', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Receipt No" value={String(form.receiptNo ?? '')} onChange={(v) => setField('receiptNo', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Gider Raporu *" resource="ExpenseReport" value={String(form.expenseReportId ?? '')} onChange={(v) => setField('expenseReportId', v)} displayField="title" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Gider Kategorisi *" resource="ExpenseCategory" value={String(form.expenseCategoryId ?? '')} onChange={(v) => setField('expenseCategoryId', v)} displayField="name" />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <Button variant="primary" appearance="outlined" onClick={onClose}>Cancel</Button>
            <Button variant="primary" appearance="filled" type="submit" disabled={mutation.isPending}>{mutation.isPending ? 'Saving…' : 'Create'}</Button>
          </div>
        </form>
      </div>
    </div>
  );
};
