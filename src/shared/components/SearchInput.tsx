import { Search } from 'lucide-react'
import { Input } from '@shared/ui/input'
import { cn } from '@lib/utils'

export function SearchInput({
  placeholder = 'Buscar...',
  className,
  ...props
}: React.ComponentProps<'input'>) {
  return (
    <div className="relative">
      <Search className="pointer-events-none absolute left-2.5 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
      <Input placeholder={placeholder} className={cn('h-8 pl-8 text-xs', className)} {...props} />
    </div>
  )
}
