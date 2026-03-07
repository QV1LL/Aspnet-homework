import { useEffect, useState } from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { selectAllCategories, type Category } from '@/features/category'
import { fetchCategories, deleteCategory } from '@/features/category'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Trash2, Plus, Edit2 } from 'lucide-react'
import type { AppDispatch } from '@/app'
import { CategoryForm } from '@/features/category'

export const AdminCategoriesPage = () => {
    const dispatch = useDispatch<AppDispatch>()
    const categories = useSelector(selectAllCategories)
    const [editingCategory, setEditingCategory] = useState<Category | null | 'new'>(null)

    useEffect(() => {
        dispatch(fetchCategories())
    }, [dispatch])

    return (
        <div className="p-6 space-y-6">
            <div className="flex justify-between items-center">
                <h1 className="text-3xl font-bold">Categories</h1>
                <Button size="sm" onClick={() => setEditingCategory('new')}>
                    <Plus className="w-4 h-4 mr-2" /> Add
                </Button>
            </div>

            {editingCategory && (
                <div className="p-4 border rounded-lg bg-muted/20 animate-in fade-in slide-in-from-top-2">
                    <h2 className="text-sm font-semibold mb-3">
                        {editingCategory === 'new' ? 'Create Category' : 'Edit Category'}
                    </h2>
                    <CategoryForm
                        category={editingCategory === 'new' ? undefined : editingCategory}
                        onCancel={() => setEditingCategory(null)}
                    />
                </div>
            )}

            <div className="border rounded-lg bg-white shadow-sm">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Category Name</TableHead>
                            <TableHead className="text-right">Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {categories.map((cat) => (
                            <TableRow key={cat.id}>
                                <TableCell className="font-medium">{cat.name}</TableCell>
                                <TableCell className="text-right space-x-1">
                                    <Button variant="ghost" size="icon" onClick={() => setEditingCategory(cat)}>
                                        <Edit2 className="w-4 h-4 text-muted-foreground" />
                                    </Button>
                                    <Button
                                        variant="ghost"
                                        size="icon"
                                        onClick={() => confirm('Delete category?') && dispatch(deleteCategory(cat.id))}
                                    >
                                        <Trash2 className="w-4 h-4 text-destructive" />
                                    </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>
        </div>
    )
}
