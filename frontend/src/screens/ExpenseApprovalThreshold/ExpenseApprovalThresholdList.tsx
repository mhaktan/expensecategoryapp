import React, { useState, useCallback, useEffect, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Button } from '@takeoff-ui/react-spar';
import { dataProvider } from '../../dataProvider';
import { useListQuery } from '../../shared/useListQuery';
import { useDeleteMutation } from '../../shared/useDeleteMutation';
import { DeleteConfirmDialog } from '../../shared/DeleteConfirmDialog';
import { ListPageLayout } from '../../shared/ListPageLayout';
import type { TableColumn } from '../../shared/ListPageLayout';
import { actionColumn } from '../../shared/ActionButtons';
import { ExpenseApprovalThresholdCreate } from './ExpenseApprovalThresholdCreate';
import { ExpenseApprovalThresholdEdit } from './ExpenseApprovalThresholdEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type ExpenseApprovalThresholdRecord = {
  id: string | number;
  name: string;
  currency: string;
  thresholdAmount: number;
  description?: string;
  isActive: boolean;
  [key: string]: unknown;
};

// ---------------------------------------------------------------------------
// Column definition — edit this array to add/remove/reorder columns
// ---------------------------------------------------------------------------
//
// Override examples:
//   • Hide a column:     remove its entry from COLUMNS
//   • Add a custom col:  { field: 'fullName', header: 'Full Name', html: (row) => `${row.firstName} ${row.lastName}` }
//   • Enable filtering:  add searchable: true  or  filterType: 'text' | 'checkbox' | 'radio' | 'datepicker'
//   • Custom cell render: html: (row) => `<span style="color:green">${row.status}</span>`
//
// Shared components (src/shared/) can be edited to change behavior globally:
//   • ListPageLayout  — table wrapper, pagination, header layout
//   • ActionButtons   — edit/delete button styles, labels, and behavior
//   • useListQuery    — data fetching, sorting, filtering logic
//   • useDeleteMutation / DeleteConfirmDialog — delete flow
//
// Action buttons override: edit src/shared/ActionButtons.ts DEFAULT_CONFIG
// or pass custom config: actionColumn('id', { hasEdit: true, config: { edit: { label: 'View', style: '...' } } })
//

const COLUMNS: TableColumn[] = [
  { field: 'id', header: 'ID', sortable: true },
  { field: 'name', header: "Name", sortable: true, searchable: true, filterType: 'text' },
  { field: 'currency', header: "Currency", sortable: true, filterType: 'radio', filterOptions: [{ label: "TRY", value: "0" }, { label: "USD", value: "1" }, { label: "EUR", value: "2" }, { label: "GBP", value: "3" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "TRY", ["1"]: "USD", ["2"]: "EUR", ["3"]: "GBP"}; return m[String(row.currency ?? '')] ?? String(row.currency ?? '\u2014'); } },
  { field: 'thresholdAmount', header: "Threshold Amount", sortable: true },
  { field: 'description', header: "Description", sortable: true, searchable: true, filterType: 'text' },
  { field: 'isActive', header: "Is Active", sortable: true, filterType: 'checkbox', filterOptions: [{ label: 'Yes', value: 'true' }, { label: 'No', value: 'false' }], html: (row: Record<string, unknown>) => row.isActive ? '<span style="color:#2e7d32;font-weight:600">Yes</span>' : '<span style="color:#999">No</span>' },
];

// ---------------------------------------------------------------------------
// ExpenseApprovalThresholdList
// ---------------------------------------------------------------------------

export const ExpenseApprovalThresholdList: React.FC = () => {
  const list = useListQuery<ExpenseApprovalThresholdRecord>({ resource: 'ExpenseApprovalThreshold' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<ExpenseApprovalThresholdRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<ExpenseApprovalThresholdRecord[]>([]);

  // ?focus=<id> ile gelindiginde kayit dogrudan acilir (My Tasks -> kayit)
  const [searchParams, setSearchParams] = useSearchParams();
  const focusId = searchParams.get('focus');
  const focusedRef = useRef<string | null>(null);
  useEffect(() => {
    if (!focusId || focusedRef.current === focusId) return;
    focusedRef.current = focusId;
    const clear = () => {
      const next = new URLSearchParams(searchParams);
      next.delete('focus');
      setSearchParams(next, { replace: true });
    };
    Promise.resolve(dataProvider.getOne('ExpenseApprovalThreshold', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as ExpenseApprovalThresholdRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('ExpenseApprovalThreshold', (ids) => { setSelectedRows([]); ids.forEach(id => triggerFlows('delete', 'ExpenseApprovalThreshold', { id })); });


  const columns = [...COLUMNS, ...actionColumn('id', { hasEdit: true, hasDelete: true })];

  const handleCrudAction = useCallback((action: string, id: string) => {
    const row = list.records.find((r) => String(r.id) === id);
    if (!row) return;
    if (action === 'edit') setEditRecord(row);
    if (action === 'delete') del.requestSingleDelete(row.id);

  }, [list.records]);

  return (
    <>
      <ListPageLayout
        title="ExpenseApprovalThreshold"
        subtitle={Object.keys(list.displayParams).length > 0 ? (
          <div style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            {Object.entries(list.displayParams).map(([k, v]) => (
              <span key={k} style={{ marginRight: 12 }}>{k}: <strong>{v}</strong></span>
            ))}
          </div>
        ) : undefined}
        records={list.records}
        columns={columns}
        dataKey="id"
        total={list.total}
        loading={list.isLoading}
        page={list.page}
        perPage={list.perPage}
        onPageChange={list.setPage}
        onPerPageChange={list.setPerPage}
        onTableRequest={list.handleTableRequest}
        selectionMode="checkbox"
        selectedRows={selectedRows}
        onSelectionChange={(rows) => setSelectedRows(rows as ExpenseApprovalThresholdRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <Button variant="danger" appearance="filled" onClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} record(s)`)}>{`Delete (${selectedRows.length})`}</Button>
          )}

          <Button variant="primary" appearance="filled" onClick={() => setShowCreate(true)}>+ Create ExpenseApprovalThreshold</Button>
        </>}
      />

      {list.isError && (
        <div style={{ padding: '10px 14px', background: '#fff3f3', border: '1px solid #f5c6c6', borderRadius: 6, color: '#c62828', fontSize: 13, marginBottom: 12 }}>
          Failed to load data: {(list.error as Error).message}
        </div>
      )}

      <DeleteConfirmDialog
        visible={!!del.deleteTarget}
        label={del.deleteTarget?.label ?? ''}
        isPending={del.isPending}
        onConfirm={del.confirmDelete}
        onCancel={() => del.setDeleteTarget(null)}
      />
      <ExpenseApprovalThresholdCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <ExpenseApprovalThresholdEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

