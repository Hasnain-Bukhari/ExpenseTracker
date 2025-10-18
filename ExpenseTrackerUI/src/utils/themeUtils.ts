export function announceThemeChange(message: string) {
  let el = document.getElementById('theme-announcer') as HTMLElement | null
  if (!el) {
    el = document.createElement('div')
    el.id = 'theme-announcer'
    el.setAttribute('aria-live', 'polite')
    el.setAttribute('aria-atomic', 'true')
    el.style.position = 'absolute'
    el.style.left = '-9999px'
    el.style.width = '1px'
    el.style.height = '1px'
    el.style.overflow = 'hidden'
    document.body.appendChild(el)
  }
  el.textContent = message
}
