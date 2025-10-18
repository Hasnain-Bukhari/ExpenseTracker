<template>
  <v-text-field
    :type="showPassword ? 'text' : 'password'"
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    v-bind="$attrs"
    variant="outlined"
    class="password-field"
    :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
    @click:append-inner="showPassword = !showPassword"
  >
    <template v-if="showStrengthMeter && modelValue" #details>
      <div class="password-strength-meter mt-2">
        <div class="strength-bar-container mb-2">
          <div 
            class="strength-bar"
            :class="strengthClass"
            :style="{ width: strengthPercentage + '%' }"
          />
        </div>
        <div class="strength-info d-flex justify-space-between align-center">
          <span class="strength-label text-caption" :class="strengthClass">
            {{ strengthLabel }}
          </span>
          <span class="text-caption text-medium-emphasis">
            {{ modelValue.length }}/8 min
          </span>
        </div>
        <div v-if="feedback.length > 0" class="strength-feedback mt-1">
          <ul class="text-caption text-medium-emphasis ma-0 pa-0">
            <li v-for="tip in feedback" :key="tip" class="feedback-item">
              {{ tip }}
            </li>
          </ul>
        </div>
      </div>
    </template>
  </v-text-field>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { validatePasswordStrength } from '@/data/mockAuth'
import type { PasswordValidation } from '@/types/auth'

interface Props {
  modelValue: string
  showStrengthMeter?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  showStrengthMeter: false
})

defineEmits<Emits>()

// Password visibility toggle
const showPassword = ref(false)

// Password strength computation
const passwordValidation = computed((): PasswordValidation => {
  return validatePasswordStrength(props.modelValue || '')
})

const strengthClass = computed(() => {
  const strength = passwordValidation.value.strength
  return {
    'strength-weak': strength === 'weak',
    'strength-medium': strength === 'medium',
    'strength-strong': strength === 'strong'
  }
})

const strengthLabel = computed(() => {
  const strength = passwordValidation.value.strength
  switch (strength) {
    case 'weak': return 'Weak'
    case 'medium': return 'Medium'
    case 'strong': return 'Strong'
    default: return ''
  }
})

const strengthPercentage = computed(() => {
  return (passwordValidation.value.score / 5) * 100
})

const feedback = computed(() => {
  return passwordValidation.value.feedback.slice(0, 3) // Show max 3 tips
})
</script>
