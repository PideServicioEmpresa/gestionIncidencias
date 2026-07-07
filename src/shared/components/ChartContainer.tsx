import { useNavigate } from 'react-router-dom'
import { ChevronRight } from 'lucide-react'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { cn } from '@lib/utils'

interface ChartContainerProps {
  title: string
  description?: string
  viewAllPath?: string
  children: React.ReactNode
  className?: string
}

export function ChartContainer({
  title,
  description,
  viewAllPath,
  children,
  className,
}: ChartContainerProps) {
  const navigate = useNavigate()

  return (
    <Card className={cn('ps-glow-card border-border/60 bg-card', className)}>
      <CardHeader className="flex-row items-start justify-between px-3 pb-2 pt-3">
        <div>
          <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            {title}
          </CardTitle>
          {description && <CardDescription className="text-[10px]">{description}</CardDescription>}
        </div>
        {viewAllPath && (
          <button
            onClick={() => navigate(viewAllPath)}
            className="flex shrink-0 items-center gap-0.5 text-xs text-primary hover:underline"
          >
            Ver todos
            <ChevronRight className="h-3 w-3" />
          </button>
        )}
      </CardHeader>
      <CardContent className="p-3 pt-0">{children}</CardContent>
    </Card>
  )
}
