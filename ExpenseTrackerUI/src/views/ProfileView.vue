<template>
  <div class="profile-page">
    <AppHeader />
    <AppNav />
    <v-main>
      <v-container fluid class="py-8">
        <v-row>
          <v-col cols="12" md="10" offset-md="1">
            <v-card class="profile-card" elevation="2">
              <v-card-title class="d-flex align-center">
                <v-icon class="me-3" color="primary">mdi-account-circle</v-icon>
                <h2 class="text-h5 font-weight-bold">Profile Settings</h2>
              </v-card-title>

              <v-card-text>
                <v-progress-linear v-if="loading" indeterminate color="primary" class="mb-4"></v-progress-linear>
                
                <v-row>
                  <!-- Profile Image Section -->
                  <v-col cols="12" md="4" class="text-center">
                    <v-avatar size="120" class="mb-4 profile-avatar">
                      <v-img
                        v-if="profile?.profileImage"
                        :src="profile.profileImage"
                        alt="Profile Image"
                        cover
                      ></v-img>
                      <v-icon v-else size="60" color="grey-lighten-1">mdi-account-circle</v-icon>
                    </v-avatar>
                    <v-btn
                      color="primary"
                      variant="outlined"
                      size="small"
                      @click="showImageDialog = true"
                      rounded="xl"
                    >
                      <v-icon start>mdi-camera</v-icon>
                      Update Photo
                    </v-btn>
                  </v-col>

                  <!-- Profile Information Section -->
                  <v-col cols="12" md="8">
                    <v-form ref="profileFormRef" v-model="profileFormValid">
                      <v-row>
                        <v-col cols="12" md="6">
                          <v-text-field
                            v-model="profileForm.fullName"
                            label="Full Name"
                            variant="outlined"
                            rounded="xl"
                            prepend-inner-icon="mdi-account"
                          ></v-text-field>
                        </v-col>
                        <v-col cols="12" md="6">
                          <v-text-field
                            v-model="profileForm.preferredName"
                            label="Preferred Name"
                            variant="outlined"
                            rounded="xl"
                            prepend-inner-icon="mdi-account-star"
                            hint="This name will be displayed on the dashboard"
                            persistent-hint
                          ></v-text-field>
                        </v-col>
                        <v-col cols="12" md="6">
                          <v-text-field
                            v-model="profileForm.phone"
                            label="Phone Number"
                            variant="outlined"
                            rounded="xl"
                            prepend-inner-icon="mdi-phone"
                          ></v-text-field>
                        </v-col>
                        <v-col cols="12" md="6">
                          <v-text-field
                            v-model="profileForm.locale"
                            label="Locale"
                            variant="outlined"
                            rounded="xl"
                            prepend-inner-icon="mdi-translate"
                            hint="e.g., en-US, es-ES"
                            persistent-hint
                          ></v-text-field>
                        </v-col>
                        <v-col cols="12" md="6">
                          <v-text-field
                            v-model="profileForm.timezone"
                            label="Timezone"
                            variant="outlined"
                            rounded="xl"
                            prepend-inner-icon="mdi-clock"
                            hint="e.g., America/New_York"
                            persistent-hint
                          ></v-text-field>
                        </v-col>
                        <v-col cols="12" md="6">
                          <v-select
                            v-model="profileForm.defaultCurrencyId"
                            :items="currencyOptions"
                            label="Default Currency"
                            variant="outlined"
                            rounded="xl"
                            prepend-inner-icon="mdi-currency-usd"
                            clearable
                            @update:model-value="onCurrencyChange"
                          ></v-select>
                        </v-col>
                        <v-col cols="12" md="6">
                          <v-select
                            v-model="profileForm.defaultAccountId"
                            :items="accountOptions"
                            label="Default Account"
                            variant="outlined"
                            rounded="xl"
                            prepend-inner-icon="mdi-bank"
                            clearable
                          ></v-select>
                        </v-col>
                      </v-row>
                    </v-form>
                  </v-col>
                </v-row>
              </v-card-text>

              <v-card-actions class="px-6 pb-6">
                <v-spacer></v-spacer>
                <v-btn
                  color="primary"
                  variant="elevated"
                  size="large"
                  :loading="saving"
                  @click="updateProfile"
                  rounded="xl"
                >
                  <v-icon start>mdi-content-save</v-icon>
                  Save Changes
                </v-btn>
              </v-card-actions>
            </v-card>

            <!-- Password Change Section -->
            <v-card class="mt-6" elevation="2" v-if="canChangePassword">
              <v-card-title class="d-flex align-center">
                <v-icon class="me-3" color="warning">mdi-key</v-icon>
                <h3 class="text-h6 font-weight-bold">Change Password</h3>
              </v-card-title>

              <v-card-text>
                <v-form ref="passwordFormRef" v-model="passwordFormValid">
                  <v-row>
                    <v-col cols="12" md="4">
                      <PasswordField
                        v-model="passwordForm.currentPassword"
                        label="Current Password"
                        :rules="currentPasswordRules"
                      />
                    </v-col>
                    <v-col cols="12" md="4">
                      <PasswordField
                        v-model="passwordForm.newPassword"
                        label="New Password"
                        :rules="newPasswordRules"
                      />
                    </v-col>
                    <v-col cols="12" md="4">
                      <PasswordField
                        v-model="passwordForm.confirmPassword"
                        label="Confirm Password"
                        :rules="confirmPasswordRules"
                      />
                    </v-col>
                  </v-row>
                </v-form>
              </v-card-text>

              <v-card-actions class="px-6 pb-6">
                <v-spacer></v-spacer>
                <v-btn
                  color="warning"
                  variant="elevated"
                  size="large"
                  :loading="changingPassword"
                  :disabled="!passwordFormValid"
                  @click="changePassword"
                  rounded="xl"
                >
                  <v-icon start>mdi-key-change</v-icon>
                  Change Password
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>

    <!-- Profile Image Update Dialog -->
    <v-dialog v-model="showImageDialog" max-width="600">
      <v-card>
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-camera" class="mr-2"></v-icon>
          Update Profile Image
        </v-card-title>
        <v-card-text>
          <div class="text-center mb-4">
            <v-avatar size="150" class="mb-4">
              <v-img
                v-if="imagePreview"
                :src="imagePreview"
                alt="Preview"
                cover
              ></v-img>
              <v-icon v-else size="60" color="grey-lighten-1">mdi-account-circle</v-icon>
            </v-avatar>
          </div>
          
          <v-file-input
            v-model="selectedFile"
            label="Choose Photo"
            accept="image/*"
            prepend-icon="mdi-camera"
            variant="outlined"
            show-size
            :rules="fileRules"
            @update:model-value="onFileSelected"
            :multiple="false"
            class="mb-4"
          >
            <template v-slot:prepend>
              <v-icon>mdi-camera</v-icon>
            </template>
          </v-file-input>

          <v-alert
            v-if="fileError"
            type="error"
            variant="tonal"
            density="compact"
            class="mb-4"
          >
            {{ fileError }}
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn @click="closeImageDialog" rounded="xl">Cancel</v-btn>
          <v-btn 
            color="primary" 
            @click="updateProfileImage"
            :loading="uploadingImage"
            :disabled="!selectedFile"
            rounded="xl"
          >
            Upload
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <AppFooter />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive, computed } from 'vue'
import { useToast } from 'vue-toastification'
import AppHeader from '@/components/Layout/AppHeader.vue'
import AppNav from '@/components/Layout/AppNav.vue'
import AppFooter from '@/components/Layout/AppFooter.vue'
import PasswordField from '@/components/auth/PasswordField.vue'
import { profileService, currencyService, accountService } from '@/services/apiService'
import type { ProfileDto, UpdateProfileDto, ChangePasswordDto } from '@/types/profile'

// Reactive data
const loading = ref(false)
const saving = ref(false)
const changingPassword = ref(false)
const profile = ref<ProfileDto | null>(null)
const currencies = ref<any[]>([])
const accounts = ref<any[]>([])
const showImageDialog = ref(false)
const selectedFile = ref<File | null>(null)
const imagePreview = ref<string | null>(null)
const uploadingImage = ref(false)
const fileError = ref<string>('')

// Form validation
const profileFormValid = ref(false)
const passwordFormValid = ref(false)

// Form refs
const profileFormRef = ref()
const passwordFormRef = ref()

// Profile form
const profileForm = reactive<UpdateProfileDto>({
  fullName: '',
  preferredName: '',
  phone: '',
  profileImage: '',
  defaultCurrencyId: null,
  defaultAccountId: null,
  locale: '',
  timezone: ''
})

// Password form
const passwordForm = reactive<ChangePasswordDto>({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// Computed properties
const canChangePassword = computed(() => {
  return profile.value?.provider === 'Local'
})

const currencyOptions = computed(() => {
  return currencies.value.map(currency => ({
    title: `${currency.code} - ${currency.name}`,
    value: currency.id
  }))
})

const accountOptions = computed(() => {
  return accounts.value.map(account => ({
    title: account.name,
    value: account.id
  }))
})

// Validation rules
const currentPasswordRules = [
  (v: string) => !!v || 'Current password is required'
]

const newPasswordRules = [
  (v: string) => !!v || 'New password is required',
  (v: string) => v.length >= 6 || 'Password must be at least 6 characters'
]

const confirmPasswordRules = [
  (v: string) => !!v || 'Please confirm your password',
  (v: string) => v === passwordForm.newPassword || 'Passwords do not match'
]

const fileRules = [
  (file: File | null) => {
    if (!file) return true
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp']
    if (!validTypes.includes(file.type)) {
      return 'Invalid file type. Please select a JPG, PNG, GIF, or WebP image.'
    }
    if (file.size > 5 * 1024 * 1024) {
      return 'File size must be less than 5MB.'
    }
    return true
  }
]

// Methods
const loadProfile = async () => {
  loading.value = true
  try {
    profile.value = await profileService.get()
    
    // Map profile data to form
    if (profile.value) {
      profileForm.fullName = profile.value.fullName || ''
      profileForm.preferredName = profile.value.preferredName || ''
      profileForm.phone = profile.value.phone || ''
      profileForm.profileImage = profile.value.profileImage || ''
      profileForm.defaultCurrencyId = profile.value.defaultCurrencyId
      profileForm.defaultAccountId = profile.value.defaultAccountId
      profileForm.locale = profile.value.locale || ''
      profileForm.timezone = profile.value.timezone || ''
      
      // Sync profile image to auth store for header display
      if (profile.value.profileImage) {
        const { useAuthStore } = await import('@/stores/auth')
        const authStore = useAuthStore()
        if (authStore.user) {
          authStore.user.avatar = profile.value.profileImage
          // Persist to localStorage
          const storedUser = localStorage.getItem('auth_user')
          if (storedUser) {
            const user = JSON.parse(storedUser)
            user.avatar = profile.value.profileImage
            localStorage.setItem('auth_user', JSON.stringify(user))
          }
        }
      }
    }
  } catch (error) {
    console.error('Failed to load profile:', error)
  } finally {
    loading.value = false
  }
}

const loadCurrencies = async () => {
  try {
    currencies.value = await currencyService.list()
  } catch (error) {
    console.error('Failed to load currencies:', error)
  }
}

const loadAccounts = async () => {
  try {
    accounts.value = await accountService.list()
  } catch (error) {
    console.error('Failed to load accounts:', error)
  }
}

const updateProfile = async () => {
  saving.value = true
  try {
    profile.value = await profileService.update(profileForm)
    
    // Save updated profile to localStorage
    if (profile.value) {
      localStorage.setItem('profile_data', JSON.stringify(profile.value))
    }
  } catch (error) {
    console.error('Failed to update profile:', error)
  } finally {
    saving.value = false
  }
}

const toast = useToast()

const changePassword = async () => {
  changingPassword.value = true
  try {
    await profileService.changePassword(passwordForm)
    // Show success toast
    toast.success('Password changed successfully!', {
      timeout: 4000
    })
    // Clear password form
    passwordForm.currentPassword = ''
    passwordForm.newPassword = ''
    passwordForm.confirmPassword = ''
    // Reset form validation
    if (passwordFormRef.value) {
      passwordFormRef.value.resetValidation()
    }
  } catch (error: any) {
    console.error('Failed to change password:', error)
    // Error toast is already shown by profileService, but show it here too for clarity
    const errorMessage = error?.response?.data?.error || error?.message || 'Failed to change password. Please try again.'
    toast.error(errorMessage, {
      timeout: 6000
    })
  } finally {
    changingPassword.value = false
  }
}

const onFileSelected = (files: File | File[] | null) => {
  fileError.value = ''
  
  // Handle single file (v-file-input can return File or File[])
  const file = Array.isArray(files) ? files[0] : files
  
  if (file) {
    // Validate file
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp']
    if (!validTypes.includes(file.type)) {
      fileError.value = 'Invalid file type. Please select a JPG, PNG, GIF, or WebP image.'
      selectedFile.value = null
      imagePreview.value = null
      return
    }
    
    if (file.size > 5 * 1024 * 1024) {
      fileError.value = 'File size must be less than 5MB.'
      selectedFile.value = null
      imagePreview.value = null
      return
    }
    
    // Create preview
    const reader = new FileReader()
    reader.onload = (e) => {
      imagePreview.value = e.target?.result as string
    }
    reader.readAsDataURL(file)
  } else {
    imagePreview.value = null
  }
}

const closeImageDialog = () => {
  showImageDialog.value = false
  selectedFile.value = null
  imagePreview.value = null
  fileError.value = ''
}

const updateProfileImage = async () => {
  if (!selectedFile.value) return
  
  uploadingImage.value = true
  fileError.value = ''
  
  try {
    const result = await profileService.updateImage(selectedFile.value)
    
    // Update local state
    // Backend returns ProfileImageDto with ImageUrl property (camelCase: imageUrl)
    const imageUrl = (result as any)?.imageUrl || (result as any)?.ImageUrl
    if (profile.value && imageUrl) {
      profile.value.profileImage = imageUrl
      profileForm.profileImage = imageUrl
      
      // Update auth store to sync with header
      const { useAuthStore } = await import('@/stores/auth')
      const authStore = useAuthStore()
      if (authStore.user) {
        authStore.user.avatar = imageUrl
        // Persist to localStorage
        const storedUser = localStorage.getItem('auth_user')
        if (storedUser) {
          const user = JSON.parse(storedUser)
          user.avatar = imageUrl
          localStorage.setItem('auth_user', JSON.stringify(user))
        }
      }
    }
    
    closeImageDialog()
  } catch (error: any) {
    console.error('Failed to update profile image:', error)
    fileError.value = error.response?.data?.error || 'Failed to upload image. Please try again.'
  } finally {
    uploadingImage.value = false
  }
}

const onCurrencyChange = async (currencyId: string) => {
  try {
    // Update the profile form with the new currency
    profileForm.defaultCurrencyId = currencyId
    
    // Load accounts filtered by the selected currency
    if (currencyId) {
      accounts.value = await profileService.getAccountsByCurrency(currencyId)
      
      // Reset the default account if it's not valid for the new currency
      const validAccountIds = accounts.value.map(acc => acc.id)
      if (profileForm.defaultAccountId && !validAccountIds.includes(profileForm.defaultAccountId)) {
        profileForm.defaultAccountId = null
      }
    }
  } catch (error) {
    console.error('Failed to load accounts for currency:', error)
  }
}

// Lifecycle
onMounted(async () => {
  // Load all data
  await Promise.all([
    loadCurrencies(),
    loadAccounts(),
    loadProfile()
  ])
})
</script>

<style scoped>
.profile-card {
  border-radius: 16px;
}

.profile-page {
  min-height: 100vh;
  background-color: rgb(var(--v-theme-background));
}
</style>