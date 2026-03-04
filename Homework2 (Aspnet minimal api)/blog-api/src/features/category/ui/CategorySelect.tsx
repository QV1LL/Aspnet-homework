import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
    type SelectSharedProps,
} from '@/components/ui/select'

export const CategorySelect = (props: SelectSharedProps) => (
    <Select {...props}>
        <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="All Categories" />
        </SelectTrigger>
        <SelectContent>
            <SelectItem value="all">All Categories</SelectItem>
            <SelectItem value="tech">Technology</SelectItem>
            <SelectItem value="lifestyle">Lifestyle</SelectItem>
        </SelectContent>
    </Select>
)
