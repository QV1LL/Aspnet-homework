import { useForm } from 'react-hook-form'
import { useDispatch } from 'react-redux'
import { createCategory, updateCategory } from '../model/actions'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import type { Category, CreateCategory } from '../model/types'
import type { AppDispatch } from '@/app'

interface Props {
    category?: Category
    onCancel: () => void
}

export const CategoryForm = ({ category, onCancel }: Props) => {
    const dispatch = useDispatch<AppDispatch>()
    const { register, handleSubmit } = useForm<CreateCategory>({
        defaultValues: category ? { name: category.name } : { name: '' },
    })

    const onSubmit = async (data: CreateCategory) => {
        if (category) {
            await dispatch(updateCategory({ ...data, id: category.id }))
        } else {
            await dispatch(createCategory(data))
        }
        onCancel()
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="flex gap-2 items-center bg-secondary/20 p-2 rounded-md">
            <Input {...register('name', { required: true })} placeholder="Category name" className="h-9" />
            <Button type="submit" size="sm">
                Save
            </Button>
            <Button type="button" variant="ghost" size="sm" onClick={onCancel}>
                Cancel
            </Button>
        </form>
    )
}
