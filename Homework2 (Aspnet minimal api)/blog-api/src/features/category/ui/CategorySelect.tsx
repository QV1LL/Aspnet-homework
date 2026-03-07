import { useEffect } from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import type { AppDispatch, RootState } from '@/app'
import { setFilters } from '@/features/post'
import { selectAllCategories } from '../model/slice'
import { fetchCategories } from '../model/actions'
import type { Category } from '../model/types'

export const CategorySelect = () => {
    const dispatch = useDispatch<AppDispatch>()

    const categories = useSelector(selectAllCategories)
    const categoryStatus = useSelector((state: RootState) => state.categories.status)

    useEffect(() => {
        if (categoryStatus === 'idle') {
            dispatch(fetchCategories())
        }
    }, [categoryStatus, dispatch])

    const handleValueChange = (value: string) => {
        dispatch(setFilters({ category: value as Category | 'All' }))
    }

    return (
        <Select onValueChange={handleValueChange}>
            <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="All Categories" />
            </SelectTrigger>
            <SelectContent>
                <SelectItem value="All">All Categories</SelectItem>
                {categories.map((category) => (
                    <SelectItem key={category.id} value={category.id}>
                        {category.name}
                    </SelectItem>
                ))}
            </SelectContent>
        </Select>
    )
}
