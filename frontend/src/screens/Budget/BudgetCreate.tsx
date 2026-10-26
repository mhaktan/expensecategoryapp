import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';

type BudgetRecord = {
  id: string | number;
  name: string;
  limitAmount: number;
  currency: string;
  periodType: string;
  startDate: string;
  endDate?: string;
  warningThresholdPercent?: number;
  userId?: string;
  expenseCategoryId: string;
};

interface BudgetCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const BudgetCreate: React.FC<BudgetCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<BudgetRecord>>({});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));

  const mutation = useMutation({
    mutationFn: (values: Partial<BudgetRecord>) => dataProvider.create('Budget', values),
    onSuccess: (_data, values) => { onSuccess(); onClose(); setForm({}); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Create Budget</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Name *" value={String(form.name ?? '')} onChange={(v) => setField('name', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Limit Amount *" value={String(form.limitAmount ?? '')} onChange={(v) => setField('limitAmount', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Currency *" value={String(form.currency ?? '')} onChange={(v) => setField('currency', v ? Number(v) : null)} searchable={false} options={[{ label: "TRY", value: "0" }, { label: "USD", value: "1" }, { label: "EUR", value: "2" }, { label: "GBP", value: "3" }, { label: "CHF", value: "4" }, { label: "JPY", value: "5" }, { label: "CAD", value: "6" }, { label: "AUD", value: "7" }, { label: "Other", value: "8" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Period Type *" value={String(form.periodType ?? '')} onChange={(v) => setField('periodType', v ? Number(v) : null)} searchable={false} options={[{ label: "Monthly", value: "0" }, { label: "Weekly", value: "1" }, { label: "Yearly", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Start Date *" value={String(form.startDate ?? '')} onChange={(v) => setField('startDate', v)} withTime />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="End Date" value={String(form.endDate ?? '')} onChange={(v) => setField('endDate', v)} withTime />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Warning Threshold Percent" value={String(form.warningThresholdPercent ?? '')} onChange={(v) => setField('warningThresholdPercent', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Kullanıcı (Sistem)" resource="User" value={String(form.userId ?? '')} onChange={(v) => setField('userId', v)} displayField="name" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Harcama Kategorisi *" resource="ExpenseCategory" value={String(form.expenseCategoryId ?? '')} onChange={(v) => setField('expenseCategoryId', v)} displayField="name" />
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
