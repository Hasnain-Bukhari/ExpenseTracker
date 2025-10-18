<template>
  <v-btn
    :color="provider.color"
    variant="outlined"
    block
    size="large"
    class="social-login-btn"
    :prepend-icon="provider.icon"
    @click="handleClick"
    :loading="loading"
    :disabled="disabled"
    :aria-label="`Sign in with ${provider.name}`"
  >
    <span>Continue with {{ provider.name }}</span>
  </v-btn>
</template>

<script setup lang="ts">
import type { SocialProvider } from '@/types/auth'

interface Props {
  provider: SocialProvider
  loading?: boolean
  disabled?: boolean
}

interface Emits {
  (e: 'click', provider: SocialProvider): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const handleClick = () => {
  emit('click', props.provider)
}
</script>
