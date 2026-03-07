import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import type { Category } from '@/features/category'
import { useNavigate } from 'react-router'

type PostCardDto = {
    id: string
    title: string
    content: string
    categories: Category[]
}

export const PostCard = ({ id, title, content, categories }: PostCardDto) => {
    const navigate = useNavigate()

    const handleReadMoreClicked = () => {
        navigate(`/post/${id}`)
    }

    return (
        <Card className="flex flex-col justify-between hover:shadow-lg transition-shadow">
            <CardHeader>
                <div className="mb-2">
                    <Badge variant="secondary">
                        {categories
                            .map((category) => category.name)
                            .reduce((acc, category) => (acc == '' ? category : `${acc}, ${category}`), '')}
                    </Badge>
                </div>
                <CardTitle className="line-clamp-2">{title}</CardTitle>
            </CardHeader>
            <CardContent>
                <p className="text-muted-foreground text-sm line-clamp-3">{content}</p>
            </CardContent>
            <CardFooter>
                <Button variant="ghost" className="w-full" onClick={handleReadMoreClicked}>
                    Read More
                </Button>
            </CardFooter>
        </Card>
    )
}
