import { ref } from 'vue'

// Singleton state - shared across all components
const showTransactionDialog = ref(false)
const showGoalDialog = ref(false)
const showBudgetDialog = ref(false)

export const useDialog = () => {
  const openTransactionDialog = () => {
    showTransactionDialog.value = true
  }

  const openGoalDialog = () => {
    showGoalDialog.value = true
  }

  const openBudgetDialog = () => {
    showBudgetDialog.value = true
  }

  const closeTransactionDialog = () => {
    showTransactionDialog.value = false
  }

  const closeGoalDialog = () => {
    showGoalDialog.value = false
  }

  const closeBudgetDialog = () => {
    showBudgetDialog.value = false
  }

  const handleQuickAdd = (action: 'transaction' | 'goal' | 'budget') => {
    if (action === 'transaction') {
      openTransactionDialog()
    } else if (action === 'goal') {
      openGoalDialog()
    } else if (action === 'budget') {
      openBudgetDialog()
    }
  }

  return {
    showTransactionDialog,
    showGoalDialog,
    showBudgetDialog,
    openTransactionDialog,
    openGoalDialog,
    openBudgetDialog,
    closeTransactionDialog,
    closeGoalDialog,
    closeBudgetDialog,
    handleQuickAdd
  }
}

