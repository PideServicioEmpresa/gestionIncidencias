import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { cn } from '@lib/utils'

interface PaginationProps {
  page: number
  totalPages: number
  onPageChange: (page: number) => void
  className?: string
}

function getPageRange(current: number, total: number): (number | 'ellipsis')[] {
  if (total <= 5) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }

  const pages: (number | 'ellipsis')[] = []

  if (current <= 3) {
    pages.push(1, 2, 3, 4, 5)
  } else if (current >= total - 2) {
    pages.push(total - 4, total - 3, total - 2, total - 1, total)
  } else {
    pages.push(current - 2, current - 1, current, current + 1, current + 2)
  }

  const result: (number | 'ellipsis')[] = []

  if ((pages[0] as number) > 1) {
    result.push(1)
    if ((pages[0] as number) > 2) result.push('ellipsis')
  }

  result.push(...pages)

  const last = pages[pages.length - 1] as number
  if (last < total) {
    if (last < total - 1) result.push('ellipsis')
    result.push(total)
  }

  return result
}

export function Pagination({ page, totalPages, onPageChange, className }: PaginationProps) {
  if (totalPages <= 1) return null

  const pages = getPageRange(page, totalPages)

  return (
    <div className={cn('flex items-center gap-1', className)}>
      <Button
        variant="outline"
        size="sm"
        className="h-7 w-7 p-0"
        onClick={() => onPageChange(1)}
        disabled={page === 1}
        aria-label="Primera pagina"
      >
        <ChevronsLeft className="h-3.5 w-3.5" />
      </Button>
      <Button
        variant="outline"
        size="sm"
        className="h-7 w-7 p-0"
        onClick={() => onPageChange(page - 1)}
        disabled={page === 1}
        aria-label="Pagina anterior"
      >
        <ChevronLeft className="h-3.5 w-3.5" />
      </Button>

      {pages.map((p, i) =>
        p === 'ellipsis' ? (
          <span
            key={`ellipsis-${i}`}
            className="flex h-7 w-7 items-center justify-center text-xs text-muted-foreground"
          >
            …
          </span>
        ) : (
          <Button
            key={p}
            variant={p === page ? 'default' : 'outline'}
            size="sm"
            className="h-7 w-7 p-0 text-xs"
            onClick={() => onPageChange(p)}
            aria-label={`Pagina ${p}`}
            aria-current={p === page ? 'page' : undefined}
          >
            {p}
          </Button>
        ),
      )}

      <Button
        variant="outline"
        size="sm"
        className="h-7 w-7 p-0"
        onClick={() => onPageChange(page + 1)}
        disabled={page === totalPages}
        aria-label="Pagina siguiente"
      >
        <ChevronRight className="h-3.5 w-3.5" />
      </Button>
      <Button
        variant="outline"
        size="sm"
        className="h-7 w-7 p-0"
        onClick={() => onPageChange(totalPages)}
        disabled={page === totalPages}
        aria-label="Ultima pagina"
      >
        <ChevronsRight className="h-3.5 w-3.5" />
      </Button>
    </div>
  )
}
