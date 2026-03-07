import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { Input } from '@/components/ui/input'
import { setFilters, selectPostsMeta } from '@/features/post'
import { Search } from 'lucide-react'
import type { AppDispatch } from '@/app'

export const PostSearch = () => {
    const dispatch = useDispatch<AppDispatch>()
    const { filters } = useSelector(selectPostsMeta)
    const [value, setValue] = useState(filters.search)

    useEffect(() => {
        const timer = setTimeout(() => {
            if (filters.search !== value) dispatch(setFilters({ search: value }))
        }, 500)

        return () => clearTimeout(timer)
    }, [value, dispatch, filters.search])

    return (
        <div className="relative w-full max-w-sm">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
                placeholder="Search posts..."
                value={value}
                onChange={(e) => setValue(e.target.value)}
                className="pl-9"
            />
        </div>
    )
}
