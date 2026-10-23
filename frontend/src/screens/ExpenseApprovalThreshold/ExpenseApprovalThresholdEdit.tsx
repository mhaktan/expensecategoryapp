import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparCheckbox, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type ExpenseApprovalThresholdRecord = {
  id: string | number;
  name: string;
  currency: string;
  thresholdAmount: number;
  description?: string;
  isActive: boolean;
};

interface ExpenseApprovalThresholdEditProps {
  record: ExpenseApprovalThresholdRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const ExpenseApprovalThresholdEdit: React.FC<ExpenseApprovalThresholdEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<ExpenseApprovalThresholdRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<ExpenseApprovalThresholdRecord>) =>
      dataProvider.update('ExpenseApprovalThreshold', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'ExpenseApprovalThreshold', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Edit ExpenseApprovalThreshold</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Name *" value={String(form.name ?? '')} onChange={(v) => setField('name', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Currency *" value={String(form.currency ?? '')} onChange={(v) => setField('currency', v ? Number(v) : null)} searchable={false} options={[{ label: "TRY", value: "0" }, { label: "USD", value: "1" }, { label: "EUR", value: "2" }, { label: "GBP", value: "3" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Threshold Amount *" value={String(form.thresholdAmount ?? '')} onChange={(v) => setField('thresholdAmount', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Description" value={String(form.description ?? '')} onChange={(v) => setField('description', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparCheckbox label="Is Active *" checked={!!form.isActive} onChange={(v) => setField('isActive', v)} />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <Button variant="primary" appearance="outlined" onClick={onClose}>Cancel</Button>
            <Button variant="primary" appearance="filled" type="submit" disabled={mutation.isPending}>{mutation.isPending ? 'Saving…' : 'Save Changes'}</Button>
          </div>
        </form>
      </div>
    </div>
  );
};
