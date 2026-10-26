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
import { BudgetCreate } from './BudgetCreate';
import { BudgetEdit } from './BudgetEdit';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

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
  { field: 'limitAmount', header: "Limit Amount", sortable: true },
  { field: 'currency', header: "Currency", sortable: true, filterType: 'radio', filterOptions: [{ label: "TRY", value: "0" }, { label: "USD", value: "1" }, { label: "EUR", value: "2" }, { label: "GBP", value: "3" }, { label: "CHF", value: "4" }, { label: "JPY", value: "5" }, { label: "CAD", value: "6" }, { label: "AUD", value: "7" }, { label: "Other", value: "8" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "TRY", ["1"]: "USD", ["2"]: "EUR", ["3"]: "GBP", ["4"]: "CHF", ["5"]: "JPY", ["6"]: "CAD", ["7"]: "AUD", ["8"]: "Other"}; return m[String(row.currency ?? '')] ?? String(row.currency ?? '\u2014'); } },
  { field: 'periodType', header: "Period Type", sortable: true, filterType: 'radio', filterOptions: [{ label: "Monthly", value: "0" }, { label: "Weekly", value: "1" }, { label: "Yearly", value: "2" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Monthly", ["1"]: "Weekly", ["2"]: "Yearly"}; return m[String(row.periodType ?? '')] ?? String(row.periodType ?? '\u2014'); } },
  { field: 'startDate', header: "Start Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.startDate ? new Date(String(row.startDate)).toLocaleDateString() : '—' },
  { field: 'endDate', header: "End Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.endDate ? new Date(String(row.endDate)).toLocaleDateString() : '—' },
  { field: 'warningThresholdPercent', header: "Warning Threshold Percent", sortable: true },
  { field: 'userId', header: "Kullanıcı (Sistem)", sortable: true },
  { field: 'expenseCategoryId', header: "Harcama Kategorisi", sortable: true },
];

// ---------------------------------------------------------------------------
// BudgetList
// ---------------------------------------------------------------------------

export const BudgetList: React.FC = () => {
  const list = useListQuery<BudgetRecord>({ resource: 'Budget' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<BudgetRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<BudgetRecord[]>([]);

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
    Promise.resolve(dataProvider.getOne('Budget', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as BudgetRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);

  const del = useDeleteMutation('Budget', (ids) => { setSelectedRows([]); });


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
        title="Budget"
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
        onSelectionChange={(rows) => setSelectedRows(rows as BudgetRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <Button variant="danger" appearance="filled" onClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} record(s)`)}>{`Delete (${selectedRows.length})`}</Button>
          )}

          <Button variant="primary" appearance="filled" onClick={() => setShowCreate(true)}>+ Create Budget</Button>
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
      <BudgetCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <BudgetEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

