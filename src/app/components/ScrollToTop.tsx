import { useEffect } from 'react'
import { useLocation } from 'react-router-dom'

export function ScrollToTop() {
  const { pathname } = useLocation()

  useEffect(() => {
    const main = document.querySelector('main')
    if (main) {
      main.scrollTop = 0
    }
    window.scrollTo(0, 0)
  }, [pathname])

  return null
}
