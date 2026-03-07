import { Skeleton } from '@/components/ui/skeleton'

export const PostDetailsSkeleton = () => (
    <div className="container max-w-3xl mx-auto py-10 space-y-8">
        <Skeleton className="h-8 w-24" />
        <div className="space-y-4">
            <div className="flex gap-2">
                <Skeleton className="h-5 w-16" />
                <Skeleton className="h-5 w-16" />
            </div>
            <Skeleton className="h-12 w-3/4" />
            <Skeleton className="h-4 w-40" />
        </div>
        <Skeleton className="aspect-video w-full rounded-xl" />
        <div className="space-y-4">
            <Skeleton className="h-4 w-full" />
            <Skeleton className="h-4 w-full" />
            <Skeleton className="h-4 w-2/3" />
        </div>
    </div>
)
