import {
  Card,
  CardContent,
  Stack,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
  InputAdornment,
  Collapse,
} from '@mui/material';
import { Search as SearchIcon, Refresh as RefreshIcon } from '@mui/icons-material';

interface FilterSectionProps {
  searchTerm: string;
  onSearchChange: (value: string) => void;
  selectedDepartment: number | '';
  onDepartmentChange: (value: number | '') => void;
  departments: Array<{ id: number; name: string }>;
  onRefresh: () => void;
}

export default function FilterSection({
  searchTerm,
  onSearchChange,
  selectedDepartment,
  onDepartmentChange,
  departments,
  onRefresh,
}: FilterSectionProps) {
  return (
    <Collapse in timeout={800}>
      <Card 
        sx={{ 
          mb: { xs: 3, sm: 4, md: 5 },
          width: "100%",
          maxWidth: { xs: "100%", sm: "100%", md: "1200px", lg: "1400px" },
          boxShadow: '0 4px 20px rgba(0,0,0,0.08)',
          borderRadius: { xs: 2, sm: 3 },
        }}
      >
        <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
          <Stack 
            direction={{ xs: 'column', sm: 'row' }} 
            spacing={{ xs: 2, sm: 3 }} 
            alignItems="center"
          >
            <TextField
              placeholder="Search employees..."
              value={searchTerm}
              onChange={(e) => onSearchChange(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon color="action" />
                  </InputAdornment>
                ),
              }}
              sx={{ 
                flexGrow: 1, 
                minWidth: { xs: "100%", sm: 200 },
                width: { xs: "100%", sm: "auto" },
              }}
              size="small"
            />
            <FormControl 
              size="small" 
              sx={{ 
                minWidth: { xs: "100%", sm: 150 },
                width: { xs: "100%", sm: "auto" },
              }}
            >
              <InputLabel>Department</InputLabel>
              <Select
                value={selectedDepartment}
                onChange={(e) => onDepartmentChange(e.target.value as number | '')}
                label="Department"
              >
                <MenuItem value="">All Departments</MenuItem>
                {departments.map((dept) => (
                  <MenuItem key={dept.id} value={dept.id}>
                    {dept.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            <IconButton 
              onClick={onRefresh}
              sx={{ 
                bgcolor: 'primary.50',
                '&:hover': { bgcolor: 'primary.100' },
                alignSelf: { xs: "center", sm: "stretch" },
              }}
            >
              <RefreshIcon />
            </IconButton>
          </Stack>
        </CardContent>
      </Card>
    </Collapse>
  );
}
