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
                    <v-avatar size="120" class="mb-4">
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
    <v-dialog v-model="showImageDialog" max-width="500">
      <v-card>
        <v-card-title>Update Profile Image</v-card-title>
        <v-card-text>
          <v-text-field
            v-model="imageUrl"
            label="Image URL"
            variant="outlined"
            hint="Enter the URL of your profile image"
            persistent-hint
          ></v-text-field>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn @click="showImageDialog = false">Cancel</v-btn>
          <v-btn color="primary" @click="updateProfileImage">Update</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <AppFooter />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive, computed } from 'vue'
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
const imageUrl = ref('')

// Form validation
const profileFormValid = ref(false)
const passwordFormValid = ref(false)

// Form refs
const profileFormRef = ref()
const passwordFormRef = ref()

// Profile form
const profileForm = reactive<UpdateProfileDto>({
  fullName: '',
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

// Methods
const loadProfile = async () => {
  loading.value = true
  try {
    profile.value = await profileService.get()
    
    // Map profile data to form
    if (profile.value) {
      profileForm.fullName = profile.value.fullName || ''
      profileForm.phone = profile.value.phone || ''
      profileForm.profileImage = profile.value.profileImage || ''
      profileForm.defaultCurrencyId = profile.value.defaultCurrencyId
      profileForm.defaultAccountId = profile.value.defaultAccountId
      profileForm.locale = profile.value.locale || ''
      profileForm.timezone = profile.value.timezone || ''
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
  } catch (error) {
    console.error('Failed to update profile:', error)
  } finally {
    saving.value = false
  }
}

const changePassword = async () => {
  changingPassword.value = true
  try {
    await profileService.changePassword(passwordForm)
    // Clear password form
    passwordForm.currentPassword = ''
    passwordForm.newPassword = ''
    passwordForm.confirmPassword = ''
  } catch (error) {
    console.error('Failed to change password:', error)
  } finally {
    changingPassword.value = false
  }
}

const updateProfileImage = async () => {
  try {
    await profileService.updateImage(imageUrl.value)
    profile.value!.profileImage = imageUrl.value
    profileForm.profileImage = imageUrl.value
    showImageDialog.value = false
    imageUrl.value = ''
  } catch (error) {
    console.error('Failed to update profile image:', error)
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
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}
</style>